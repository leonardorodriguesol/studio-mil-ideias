namespace StudioMilIdeias.Application.DTOs.Products;

public sealed class ProductDetailsResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public bool IsActive { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public string DigitalFileUrl { get; init; } = string.Empty;
}
