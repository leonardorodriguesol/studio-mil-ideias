using Microsoft.EntityFrameworkCore;
using StudioMilIdeias.Application.Abstractions;
using StudioMilIdeias.Application.DTOs.Orders;
using StudioMilIdeias.Domain.Entities;
using StudioMilIdeias.Domain.Enums;

namespace StudioMilIdeias.Infrastructure.Persistence;

public sealed class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _dbContext;

    public OrderRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        return _dbContext.Orders.AddAsync(order, cancellationToken).AsTask();
    }

    public Task AddItemsAsync(IEnumerable<OrderItem> items, CancellationToken cancellationToken = default)
    {
        return _dbContext.OrderItems.AddRangeAsync(items, cancellationToken);
    }

    public Task<Order?> GetByPaymentIdTrackedAsync(
        string paymentId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Orders.FirstOrDefaultAsync(x => x.PaymentId == paymentId, cancellationToken);
    }

    public async Task<IReadOnlyList<OrderListItemResponse>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.Orders
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new OrderListItemResponse
            {
                Id = x.Id,
                TotalAmount = x.TotalAmount,
                Status = x.Status.ToString(),
                PaymentProvider = x.PaymentProvider,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return items;
    }

    public async Task<OrderDetailsResponse?> GetByIdAndUserIdAsync(
        Guid orderId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var order = await _dbContext.Orders
            .AsNoTracking()
            .Where(x => x.Id == orderId && x.UserId == userId)
            .Select(x => new
            {
                x.Id,
                x.TotalAmount,
                Status = x.Status.ToString(),
                x.PaymentProvider,
                x.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (order is null)
        {
            return null;
        }

        var orderItems = await _dbContext.OrderItems
            .AsNoTracking()
            .Where(x => x.OrderId == orderId)
            .OrderBy(x => x.Id)
            .Select(x => new OrderItemResponse
            {
                ProductId = x.ProductId,
                ProductName = x.Product != null ? x.Product.Name : string.Empty,
                ProductSlug = x.Product != null ? x.Product.Slug : string.Empty,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                TotalPrice = x.UnitPrice * x.Quantity
            })
            .ToListAsync(cancellationToken);

        return new OrderDetailsResponse
        {
            Id = order.Id,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            PaymentProvider = order.PaymentProvider,
            CreatedAt = order.CreatedAt,
            Items = orderItems
        };
    }

    public Task<PaidOrderItemAccess?> GetPaidOrderItemAccessAsync(
        Guid userId,
        Guid orderId,
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.OrderItems
            .AsNoTracking()
            .Where(x => x.OrderId == orderId && x.ProductId == productId)
            .Where(x => x.Order != null
                && x.Order.UserId == userId
                && x.Order.Status == OrderStatus.Paid)
            .Select(x => new PaidOrderItemAccess
            {
                UserId = userId,
                OrderId = orderId,
                ProductId = productId,
                ProductName = x.Product != null ? x.Product.Name : string.Empty,
                DigitalFileUrl = x.Product != null ? x.Product.DigitalFileUrl : string.Empty
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
