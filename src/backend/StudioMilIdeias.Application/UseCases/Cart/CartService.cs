using StudioMilIdeias.Application.Abstractions;
using StudioMilIdeias.Application.DTOs.Cart;
using StudioMilIdeias.Domain.Entities;
using DomainCart = StudioMilIdeias.Domain.Entities.Cart;

namespace StudioMilIdeias.Application.UseCases.Cart;

public sealed class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IApplicationDbContext _dbContext;

    public CartService(
        ICartRepository cartRepository,
        IProductRepository productRepository,
        IApplicationDbContext dbContext)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _dbContext = dbContext;
    }

    public async Task<CartResponse> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
        return cart ?? CreateEmptyCart(userId);
    }

    public async Task<CartResponse> AddItemAsync(
        Guid userId,
        AddCartItemRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException("Quantity must be greater than zero.");
        }

        var product = await _productRepository.GetByIdTrackedAsync(request.ProductId, cancellationToken);
        if (product is null || !product.IsActive)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        var cart = await _cartRepository.GetByUserIdTrackedAsync(userId, cancellationToken);
        if (cart is null)
        {
            cart = new DomainCart
            {
                Id = Guid.NewGuid(),
                UserId = userId
            };

            await _cartRepository.AddCartAsync(cart, cancellationToken);
        }

        var existingItem = await _cartRepository.GetItemByCartAndProductTrackedAsync(
            cart.Id,
            request.ProductId,
            cancellationToken);

        if (existingItem is null)
        {
            var cartItem = new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                ProductId = product.Id,
                Quantity = request.Quantity,
                UnitPrice = product.Price
            };

            await _cartRepository.AddItemAsync(cartItem, cancellationToken);
        }
        else
        {
            existingItem.Quantity += request.Quantity;
            existingItem.UnitPrice = product.Price;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var updatedCart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
        return updatedCart ?? CreateEmptyCart(userId);
    }

    public async Task RemoveItemAsync(
        Guid userId,
        Guid cartItemId,
        CancellationToken cancellationToken = default)
    {
        var existingItem = await _cartRepository.GetItemByIdAndUserTrackedAsync(
            cartItemId,
            userId,
            cancellationToken);

        if (existingItem is null)
        {
            throw new KeyNotFoundException("Cart item not found.");
        }

        _cartRepository.RemoveItem(existingItem);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static CartResponse CreateEmptyCart(Guid userId)
    {
        return new CartResponse
        {
            Id = Guid.Empty,
            UserId = userId,
            TotalAmount = 0,
            Items = []
        };
    }
}
