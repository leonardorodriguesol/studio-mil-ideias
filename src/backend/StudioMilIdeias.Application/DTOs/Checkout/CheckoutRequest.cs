namespace StudioMilIdeias.Application.DTOs.Checkout;

public sealed class CheckoutRequest
{
    public string SuccessUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
}
