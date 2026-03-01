using StudioMilIdeias.Application.DTOs.Orders;

namespace StudioMilIdeias.Application.UseCases.Orders;

public interface IOrderQueries
{
    Task<IReadOnlyList<OrderListItemResponse>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<OrderDetailsResponse?> GetByIdAndUserIdAsync(
        Guid orderId,
        Guid userId,
        CancellationToken cancellationToken = default);
}
