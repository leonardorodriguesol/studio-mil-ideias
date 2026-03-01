namespace StudioMilIdeias.Application.DTOs.Cart;

public sealed class AddCartItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}
