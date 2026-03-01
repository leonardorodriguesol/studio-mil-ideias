using StudioMilIdeias.Domain.Entities;

namespace StudioMilIdeias.Application.Abstractions;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    Task AddItemsAsync(IEnumerable<OrderItem> items, CancellationToken cancellationToken = default);
    Task<Order?> GetByPaymentIdTrackedAsync(string paymentId, CancellationToken cancellationToken = default);
}
