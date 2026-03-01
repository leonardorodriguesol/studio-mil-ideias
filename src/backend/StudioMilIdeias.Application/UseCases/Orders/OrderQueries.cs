using StudioMilIdeias.Application.Abstractions;
using StudioMilIdeias.Application.DTOs.Orders;

namespace StudioMilIdeias.Application.UseCases.Orders;

public sealed class OrderQueries : IOrderQueries
{
    private readonly IOrderRepository _orderRepository;

    public OrderQueries(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public Task<IReadOnlyList<OrderListItemResponse>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _orderRepository.GetByUserIdAsync(userId, cancellationToken);
    }

    public Task<OrderDetailsResponse?> GetByIdAndUserIdAsync(
        Guid orderId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _orderRepository.GetByIdAndUserIdAsync(orderId, userId, cancellationToken);
    }
}
