namespace StudioMilIdeias.Application.Abstractions;

public sealed class CheckoutSessionCreateItem
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public sealed class CheckoutSessionCreateInput
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public string SuccessUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
    public IReadOnlyList<CheckoutSessionCreateItem> Items { get; set; } = [];
}

public sealed class CheckoutSessionCreateResult
{
    public string SessionId { get; set; } = string.Empty;
    public string CheckoutUrl { get; set; } = string.Empty;
}

public interface ICheckoutPaymentGateway
{
    Task<CheckoutSessionCreateResult> CreateCheckoutSessionAsync(
        CheckoutSessionCreateInput input,
        CancellationToken cancellationToken = default);

    string? ReadCompletedCheckoutSessionId(string payload, string signatureHeader);
}
