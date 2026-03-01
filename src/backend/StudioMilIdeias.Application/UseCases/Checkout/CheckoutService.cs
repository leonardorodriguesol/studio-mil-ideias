using StudioMilIdeias.Application.Abstractions;
using StudioMilIdeias.Application.DTOs.Checkout;
using StudioMilIdeias.Domain.Entities;
using StudioMilIdeias.Domain.Enums;

namespace StudioMilIdeias.Application.UseCases.Checkout;

public sealed class CheckoutService : ICheckoutService
{
    private readonly ICartRepository _cartRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICheckoutPaymentGateway _checkoutPaymentGateway;
    private readonly IApplicationDbContext _dbContext;

    public CheckoutService(
        ICartRepository cartRepository,
        IOrderRepository orderRepository,
        ICheckoutPaymentGateway checkoutPaymentGateway,
        IApplicationDbContext dbContext)
    {
        _cartRepository = cartRepository;
        _orderRepository = orderRepository;
        _checkoutPaymentGateway = checkoutPaymentGateway;
        _dbContext = dbContext;
    }

    public async Task<CheckoutResponse> CreateAsync(
        Guid userId,
        CheckoutRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateUrls(request.SuccessUrl, request.CancelUrl);

        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
        if (cart is null || cart.Items.Count == 0)
        {
            throw new InvalidOperationException("Cart is empty.");
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TotalAmount = cart.TotalAmount,
            Status = OrderStatus.Pending,
            PaymentProvider = "Stripe",
            PaymentId = string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        var session = await _checkoutPaymentGateway.CreateCheckoutSessionAsync(
            new CheckoutSessionCreateInput
            {
                OrderId = order.Id,
                UserId = userId,
                SuccessUrl = request.SuccessUrl.Trim(),
                CancelUrl = request.CancelUrl.Trim(),
                Items = cart.Items
                    .Select(x => new CheckoutSessionCreateItem
                    {
                        Name = x.ProductName,
                        Quantity = x.Quantity,
                        UnitPrice = x.UnitPrice
                    })
                    .ToList()
            },
            cancellationToken);

        order.PaymentId = session.SessionId;

        var orderItems = cart.Items
            .Select(x => new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice
            })
            .ToList();

        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.AddItemsAsync(orderItems, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CheckoutResponse
        {
            OrderId = order.Id,
            SessionId = session.SessionId,
            CheckoutUrl = session.CheckoutUrl
        };
    }

    public async Task HandleStripeWebhookAsync(
        string payload,
        string signatureHeader,
        CancellationToken cancellationToken = default)
    {
        var sessionId = _checkoutPaymentGateway.ReadCompletedCheckoutSessionId(payload, signatureHeader);
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return;
        }

        var order = await _orderRepository.GetByPaymentIdTrackedAsync(sessionId, cancellationToken);
        if (order is null || order.Status == OrderStatus.Paid)
        {
            return;
        }

        order.Status = OrderStatus.Paid;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void ValidateUrls(string successUrl, string cancelUrl)
    {
        if (!Uri.TryCreate(successUrl?.Trim(), UriKind.Absolute, out _))
        {
            throw new InvalidOperationException("SuccessUrl must be a valid absolute URL.");
        }

        if (!Uri.TryCreate(cancelUrl?.Trim(), UriKind.Absolute, out _))
        {
            throw new InvalidOperationException("CancelUrl must be a valid absolute URL.");
        }
    }
}
