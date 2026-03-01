using StudioMilIdeias.Application.DTOs.Products;

namespace StudioMilIdeias.Application.Abstractions;

public interface IProductRepository
{
    Task<IReadOnlyList<ProductListItemResponse>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<ProductDetailsResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
