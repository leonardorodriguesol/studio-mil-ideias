using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using StudioMilIdeias.Application.Abstractions;

namespace StudioMilIdeias.Infrastructure.Payments;

public sealed class StripeCheckoutPaymentGateway : ICheckoutPaymentGateway
{
    private readonly StripeOptions _options;

    public StripeCheckoutPaymentGateway(IOptions<StripeOptions> options)
    {
        _options = options.Value;
    }

    public async Task<CheckoutSessionCreateResult> CreateCheckoutSessionAsync(
        CheckoutSessionCreateInput input,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.SecretKey))
        {
            throw new InvalidOperationException("Stripe secret key is not configured.");
        }

        var items = input.Items
            .Where(x => x.Quantity > 0 && x.UnitPrice > 0 && !string.IsNullOrWhiteSpace(x.Name))
            .Select(x => new SessionLineItemOptions
            {
                Quantity = x.Quantity,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = _options.Currency.ToLowerInvariant(),
                    UnitAmount = ToMinorUnits(x.UnitPrice),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = x.Name
                    }
                }
            })
            .ToList();

        if (items.Count == 0)
        {
            throw new InvalidOperationException("Checkout requires at least one valid line item.");
        }

        StripeConfiguration.ApiKey = _options.SecretKey;

        var sessionService = new SessionService();
        var session = await sessionService.CreateAsync(
            new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = input.SuccessUrl,
                CancelUrl = input.CancelUrl,
                ClientReferenceId = input.OrderId.ToString(),
                Metadata = new Dictionary<string, string>
                {
                    ["order_id"] = input.OrderId.ToString(),
                    ["user_id"] = input.UserId.ToString()
                },
                LineItems = items
            },
            cancellationToken: cancellationToken);

        if (string.IsNullOrWhiteSpace(session.Id) || string.IsNullOrWhiteSpace(session.Url))
        {
            throw new InvalidOperationException("Stripe did not return a valid checkout session.");
        }

        return new CheckoutSessionCreateResult
        {
            SessionId = session.Id,
            CheckoutUrl = session.Url
        };
    }

    public string? ReadCompletedCheckoutSessionId(string payload, string signatureHeader)
    {
        if (string.IsNullOrWhiteSpace(_options.WebhookSecret))
        {
            throw new InvalidOperationException("Stripe webhook secret is not configured.");
        }

        if (string.IsNullOrWhiteSpace(signatureHeader))
        {
            throw new InvalidOperationException("Missing Stripe-Signature header.");
        }

        var stripeEvent = EventUtility.ConstructEvent(payload, signatureHeader, _options.WebhookSecret);
        var isCompleted = stripeEvent.Type == EventTypes.CheckoutSessionCompleted;
        var isAsyncPaymentSucceeded = stripeEvent.Type == EventTypes.CheckoutSessionAsyncPaymentSucceeded;
        if (!isCompleted && !isAsyncPaymentSucceeded)
        {
            return null;
        }

        var session = stripeEvent.Data.Object as Session;
        if (session is null || string.IsNullOrWhiteSpace(session.Id))
        {
            return null;
        }

        if (isCompleted && !string.Equals(session.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return session.Id;
    }

    private static long ToMinorUnits(decimal amount)
    {
        return Convert.ToInt64(decimal.Round(amount * 100m, 0, MidpointRounding.AwayFromZero));
    }
}
