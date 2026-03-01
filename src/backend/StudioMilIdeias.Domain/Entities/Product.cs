namespace StudioMilIdeias.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Guid CategoryId { get; set; }
    public string DigitalFileUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public Category? Category { get; set; }
}
