using StudioMilIdeias.Domain.Enums;

namespace StudioMilIdeias.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string PaymentProvider { get; set; } = string.Empty;
    public string PaymentId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
}
