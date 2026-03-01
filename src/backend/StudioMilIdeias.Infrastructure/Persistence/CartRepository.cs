using Microsoft.EntityFrameworkCore;
using StudioMilIdeias.Application.Abstractions;
using StudioMilIdeias.Application.DTOs.Cart;
using StudioMilIdeias.Domain.Entities;

namespace StudioMilIdeias.Infrastructure.Persistence;

public sealed class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CartRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CartResponse?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cart = await _dbContext.Carts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (cart is null)
        {
            return null;
        }

        var items = await _dbContext.CartItems
            .AsNoTracking()
            .Where(x => x.CartId == cart.Id)
            .OrderBy(x => x.Id)
            .Select(x => new CartItemResponse
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductName = x.Product != null ? x.Product.Name : string.Empty,
                ProductSlug = x.Product != null ? x.Product.Slug : string.Empty,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                TotalPrice = x.UnitPrice * x.Quantity
            })
            .ToListAsync(cancellationToken);

        return new CartResponse
        {
            Id = cart.Id,
            UserId = cart.UserId,
            TotalAmount = items.Sum(x => x.TotalPrice),
            Items = items
        };
    }

    public Task<Cart?> GetByUserIdTrackedAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Carts.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }

    public Task<CartItem?> GetItemByCartAndProductTrackedAsync(
        Guid cartId,
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.CartItems.FirstOrDefaultAsync(
            x => x.CartId == cartId && x.ProductId == productId,
            cancellationToken);
    }

    public Task<CartItem?> GetItemByIdAndUserTrackedAsync(
        Guid cartItemId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.CartItems
            .Include(x => x.Cart)
            .FirstOrDefaultAsync(
                x => x.Id == cartItemId && x.Cart != null && x.Cart.UserId == userId,
                cancellationToken);
    }

    public Task AddCartAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        return _dbContext.Carts.AddAsync(cart, cancellationToken).AsTask();
    }

    public Task AddItemAsync(CartItem cartItem, CancellationToken cancellationToken = default)
    {
        return _dbContext.CartItems.AddAsync(cartItem, cancellationToken).AsTask();
    }

    public void RemoveItem(CartItem cartItem)
    {
        _dbContext.CartItems.Remove(cartItem);
    }
}
