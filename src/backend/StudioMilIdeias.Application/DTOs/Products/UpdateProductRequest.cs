namespace StudioMilIdeias.Application.DTOs.Products;

public sealed class UpdateProductRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Slug { get; init; } = string.Empty;
    public bool IsActive { get; init; } = true;
    public Guid CategoryId { get; init; }
    public string DigitalFileUrl { get; init; } = string.Empty;
}
