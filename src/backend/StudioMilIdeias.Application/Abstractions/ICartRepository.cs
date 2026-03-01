using StudioMilIdeias.Application.DTOs.Cart;
using StudioMilIdeias.Domain.Entities;

namespace StudioMilIdeias.Application.Abstractions;

public interface ICartRepository
{
    Task<CartResponse?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Cart?> GetByUserIdTrackedAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<CartItem?> GetItemByCartAndProductTrackedAsync(
        Guid cartId,
        Guid productId,
        CancellationToken cancellationToken = default);
    Task<CartItem?> GetItemByIdAndUserTrackedAsync(
        Guid cartItemId,
        Guid userId,
        CancellationToken cancellationToken = default);
    Task AddCartAsync(Cart cart, CancellationToken cancellationToken = default);
    Task AddItemAsync(CartItem cartItem, CancellationToken cancellationToken = default);
    void RemoveItem(CartItem cartItem);
}
