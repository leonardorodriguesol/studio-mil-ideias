using StudioMilIdeias.Application.Abstractions;
using StudioMilIdeias.Application.DTOs.Products;
using StudioMilIdeias.Application.UseCases.Products;
using StudioMilIdeias.Domain.Entities;

namespace StudioMilIdeias.Application.Tests;

public sealed class ProductQueriesTests
{
    [Fact]
    public async Task GetByIdAsync_ShouldReturnRepositoryResponse()
    {
        var expectedId = Guid.NewGuid();
        var repository = new FakeProductRepository
        {
            ProductToReturn = new ProductDetailsResponse
            {
                Id = expectedId,
                Name = "Product A",
                Slug = "product-a",
                Description = "desc",
                Price = 10m,
                IsActive = true,
                CategoryName = "Design",
                DigitalFileUrl = "s3://private/file.zip"
            }
        };
        var sut = new ProductQueries(repository);

        var result = await sut.GetByIdAsync(expectedId);

        Assert.NotNull(result);
        Assert.Equal(expectedId, result.Id);
        Assert.Equal(expectedId, repository.LastRequestedId);
        Assert.True(repository.GetByIdCalled);
    }

    [Fact]
    public async Task GetAllActiveAsync_ShouldReturnRepositoryResponse()
    {
        var repository = new FakeProductRepository
        {
            ActiveProductsToReturn =
            [
                new ProductListItemResponse
                {
                    Id = Guid.NewGuid(),
                    Name = "Product A",
                    Slug = "product-a",
                    Price = 10m,
                    CategoryName = "Design"
                }
            ]
        };
        var sut = new ProductQueries(repository);

        var result = await sut.GetAllActiveAsync();

        Assert.Single(result);
        Assert.True(repository.GetAllCalled);
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        public IReadOnlyList<ProductListItemResponse> ActiveProductsToReturn { get; set; } = [];
        public ProductDetailsResponse? ProductToReturn { get; set; }
        public Guid? LastRequestedId { get; private set; }
        public bool GetAllCalled { get; private set; }
        public bool GetByIdCalled { get; private set; }

        public Task<IReadOnlyList<ProductListItemResponse>> GetAllActiveAsync(
            CancellationToken cancellationToken = default)
        {
            GetAllCalled = true;
            return Task.FromResult(ActiveProductsToReturn);
        }

        public Task<ProductDetailsResponse?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            GetByIdCalled = true;
            LastRequestedId = id;
            return Task.FromResult(ProductToReturn);
        }

        public Task<ProductDetailsResponse?> GetByIdAnyAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<ProductDetailsResponse?>(null);
        }

        public Task<Product?> GetByIdTrackedAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Product?>(null);
        }

        public Task<bool> ExistsBySlugAsync(
            string slug,
            Guid? excludeProductId = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Remove(Product product)
        {
        }
    }
}
