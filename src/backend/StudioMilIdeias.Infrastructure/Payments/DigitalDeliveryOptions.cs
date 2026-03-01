namespace StudioMilIdeias.Infrastructure.Payments;

public sealed class DigitalDeliveryOptions
{
    public const string SectionName = "DigitalDelivery";

    public string TokenSecret { get; set; } = string.Empty;
    public int TokenTtlMinutes { get; set; } = 15;
}
