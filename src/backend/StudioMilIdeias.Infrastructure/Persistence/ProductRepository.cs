using Microsoft.EntityFrameworkCore;
using StudioMilIdeias.Application.Abstractions;
using StudioMilIdeias.Application.DTOs.Products;
using StudioMilIdeias.Domain.Entities;

namespace StudioMilIdeias.Infrastructure.Persistence;

public sealed class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProductListItemResponse>> GetAllActiveAsync(
        CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.Products
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ProductListItemResponse
            {
                Id = x.Id,
                Name = x.Name,
                Slug = x.Slug,
                Price = x.Price,
                CategoryName = x.Category != null ? x.Category.Name : string.Empty
            })
            .ToListAsync(cancellationToken);

        return items;
    }

    public Task<ProductDetailsResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Products
            .AsNoTracking()
            .Where(x => x.Id == id && x.IsActive)
            .Select(x => new ProductDetailsResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Slug = x.Slug,
                Price = x.Price,
                IsActive = x.IsActive,
                CategoryName = x.Category != null ? x.Category.Name : string.Empty,
                DigitalFileUrl = x.DigitalFileUrl
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<ProductDetailsResponse?> GetByIdAnyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Products
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new ProductDetailsResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Slug = x.Slug,
                Price = x.Price,
                IsActive = x.IsActive,
                CategoryName = x.Category != null ? x.Category.Name : string.Empty,
                DigitalFileUrl = x.DigitalFileUrl
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Product?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> ExistsBySlugAsync(
        string slug,
        Guid? excludeProductId = null,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Products.AnyAsync(
            x => x.Slug == slug && (!excludeProductId.HasValue || x.Id != excludeProductId.Value),
            cancellationToken);
    }

    public Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        return _dbContext.Products.AddAsync(product, cancellationToken).AsTask();
    }

    public void Remove(Product product)
    {
        _dbContext.Products.Remove(product);
    }
}
