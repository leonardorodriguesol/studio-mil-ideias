namespace StudioMilIdeias.Application.DTOs.Checkout;

public sealed class CheckoutResponse
{
    public Guid OrderId { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string CheckoutUrl { get; set; } = string.Empty;
}
