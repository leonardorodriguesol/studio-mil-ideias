using StudioMilIdeias.Application.Abstractions;
using StudioMilIdeias.Application.DTOs.Products;
using StudioMilIdeias.Domain.Entities;

namespace StudioMilIdeias.Application.UseCases.Products;

public sealed class ProductManagementService : IProductManagementService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IApplicationDbContext _dbContext;

    public ProductManagementService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IApplicationDbContext dbContext)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _dbContext = dbContext;
    }

    public async Task<ProductDetailsResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidateProductDataAsync(
            request.Name,
            request.Price,
            request.Slug,
            request.CategoryId,
            null,
            cancellationToken);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Price = request.Price,
            Slug = request.Slug.Trim().ToLowerInvariant(),
            IsActive = request.IsActive,
            CategoryId = request.CategoryId,
            DigitalFileUrl = request.DigitalFileUrl.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _productRepository.AddAsync(product, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var created = await _productRepository.GetByIdAnyAsync(product.Id, cancellationToken);
        return created ?? throw new InvalidOperationException("Failed to load created product.");
    }

    public async Task<ProductDetailsResponse> UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdTrackedAsync(id, cancellationToken);
        if (product is null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        await ValidateProductDataAsync(
            request.Name,
            request.Price,
            request.Slug,
            request.CategoryId,
            id,
            cancellationToken);

        product.Name = request.Name.Trim();
        product.Description = request.Description.Trim();
        product.Price = request.Price;
        product.Slug = request.Slug.Trim().ToLowerInvariant();
        product.IsActive = request.IsActive;
        product.CategoryId = request.CategoryId;
        product.DigitalFileUrl = request.DigitalFileUrl.Trim();

        await _dbContext.SaveChangesAsync(cancellationToken);

        var updated = await _productRepository.GetByIdAnyAsync(id, cancellationToken);
        return updated ?? throw new InvalidOperationException("Failed to load updated product.");
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdTrackedAsync(id, cancellationToken);
        if (product is null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        _productRepository.Remove(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateProductDataAsync(
        string name,
        decimal price,
        string slug,
        Guid categoryId,
        Guid? excludeProductId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Product name is required.");
        }

        if (price <= 0)
        {
            throw new InvalidOperationException("Price must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new InvalidOperationException("Slug is required.");
        }

        var normalizedSlug = slug.Trim().ToLowerInvariant();
        var slugExists = await _productRepository.ExistsBySlugAsync(
            normalizedSlug,
            excludeProductId,
            cancellationToken);
        if (slugExists)
        {
            throw new InvalidOperationException("Slug is already in use.");
        }

        var categoryExists = await _categoryRepository.ExistsByIdAsync(categoryId, cancellationToken);
        if (!categoryExists)
        {
            throw new InvalidOperationException("Category not found.");
        }
    }
}
