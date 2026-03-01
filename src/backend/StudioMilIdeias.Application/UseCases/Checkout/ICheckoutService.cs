using StudioMilIdeias.Application.DTOs.Checkout;

namespace StudioMilIdeias.Application.UseCases.Checkout;

public interface ICheckoutService
{
    Task<CheckoutResponse> CreateAsync(
        Guid userId,
        CheckoutRequest request,
        CancellationToken cancellationToken = default);

    Task HandleStripeWebhookAsync(
        string payload,
        string signatureHeader,
        CancellationToken cancellationToken = default);
}
