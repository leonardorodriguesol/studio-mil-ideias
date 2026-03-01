using System.Text.Json;
using StudioMilIdeias.Application.Abstractions;

namespace StudioMilIdeias.Api.Tests.Integration.Infrastructure;

public sealed class FakeCheckoutPaymentGateway : ICheckoutPaymentGateway
{
    private const string SignatureHeaderValue = "test-signature";

    public Task<CheckoutSessionCreateResult> CreateCheckoutSessionAsync(
        CheckoutSessionCreateInput input,
        CancellationToken cancellationToken = default)
    {
        var sessionId = $"test_session_{input.OrderId:N}";
        return Task.FromResult(new CheckoutSessionCreateResult
        {
            SessionId = sessionId,
            CheckoutUrl = $"https://checkout.tests.local/{sessionId}"
        });
    }

    public string? ReadCompletedCheckoutSessionId(string payload, string signatureHeader)
    {
        if (!string.Equals(signatureHeader, SignatureHeaderValue, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Invalid Stripe-Signature header.");
        }

        if (string.IsNullOrWhiteSpace(payload))
        {
            return null;
        }

        using var json = JsonDocument.Parse(payload);
        if (!json.RootElement.TryGetProperty("sessionId", out var sessionIdProperty))
        {
            return null;
        }

        return sessionIdProperty.GetString();
    }
}
