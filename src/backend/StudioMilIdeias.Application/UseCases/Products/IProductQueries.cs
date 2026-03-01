using StudioMilIdeias.Application.DTOs.Products;

namespace StudioMilIdeias.Application.UseCases.Products;

public interface IProductQueries
{
    Task<IReadOnlyList<ProductListItemResponse>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<ProductDetailsResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
