using StudioMilIdeias.Application.DTOs.Cart;

namespace StudioMilIdeias.Application.UseCases.Cart;

public interface ICartService
{
    Task<CartResponse> GetAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<CartResponse> AddItemAsync(
        Guid userId,
        AddCartItemRequest request,
        CancellationToken cancellationToken = default);
    Task RemoveItemAsync(Guid userId, Guid cartItemId, CancellationToken cancellationToken = default);
}
