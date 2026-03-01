using Microsoft.EntityFrameworkCore;
using StudioMilIdeias.Application.Abstractions;
using StudioMilIdeias.Application.DTOs.Products;

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
}
