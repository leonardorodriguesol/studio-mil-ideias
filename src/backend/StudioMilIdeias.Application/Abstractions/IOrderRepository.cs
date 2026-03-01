using StudioMilIdeias.Application.DTOs.Orders;
using StudioMilIdeias.Domain.Entities;

namespace StudioMilIdeias.Application.Abstractions;

public sealed class PaidOrderItemAccess
{
    public Guid UserId { get; init; }
    public Guid OrderId { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string DigitalFileUrl { get; init; } = string.Empty;
}

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    Task AddItemsAsync(IEnumerable<OrderItem> items, CancellationToken cancellationToken = default);
    Task<Order?> GetByPaymentIdTrackedAsync(string paymentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderListItemResponse>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
    Task<OrderDetailsResponse?> GetByIdAndUserIdAsync(
        Guid orderId,
        Guid userId,
        CancellationToken cancellationToken = default);
    Task<PaidOrderItemAccess?> GetPaidOrderItemAccessAsync(
        Guid userId,
        Guid orderId,
        Guid productId,
        CancellationToken cancellationToken = default);
}
