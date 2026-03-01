namespace StudioMilIdeias.Application.DTOs.Products;

public sealed class ProductListItemResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string CategoryName { get; init; } = string.Empty;
}
