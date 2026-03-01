namespace StudioMilIdeias.Application.DTOs.Orders;

public sealed class OrderDetailsResponse
{
    public Guid Id { get; init; }
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = string.Empty;
    public string PaymentProvider { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public IReadOnlyList<OrderItemResponse> Items { get; init; } = [];
}
