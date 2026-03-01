using StudioMilIdeias.Application.Abstractions;
using StudioMilIdeias.Application.DTOs.Products;

namespace StudioMilIdeias.Application.UseCases.Products;

public sealed class ProductQueries : IProductQueries
{
    private readonly IProductRepository _productRepository;

    public ProductQueries(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public Task<IReadOnlyList<ProductListItemResponse>> GetAllActiveAsync(
        CancellationToken cancellationToken = default)
    {
        return _productRepository.GetAllActiveAsync(cancellationToken);
    }

    public Task<ProductDetailsResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _productRepository.GetByIdAsync(id, cancellationToken);
    }
}
