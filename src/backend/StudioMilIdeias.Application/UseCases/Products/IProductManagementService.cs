using StudioMilIdeias.Application.DTOs.Products;

namespace StudioMilIdeias.Application.UseCases.Products;

public interface IProductManagementService
{
    Task<ProductDetailsResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<ProductDetailsResponse> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
