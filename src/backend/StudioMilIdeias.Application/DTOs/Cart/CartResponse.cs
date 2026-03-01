namespace StudioMilIdeias.Application.DTOs.Cart;

public sealed class CartResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public IReadOnlyList<CartItemResponse> Items { get; set; } = [];
}
