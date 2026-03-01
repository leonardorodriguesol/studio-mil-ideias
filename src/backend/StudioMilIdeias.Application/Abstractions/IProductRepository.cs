using StudioMilIdeias.Application.DTOs.Products;
using StudioMilIdeias.Domain.Entities;

namespace StudioMilIdeias.Application.Abstractions;

public interface IProductRepository
{
    Task<IReadOnlyList<ProductListItemResponse>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<ProductDetailsResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductDetailsResponse?> GetByIdAnyAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySlugAsync(string slug, Guid? excludeProductId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    void Remove(Product product);
}
