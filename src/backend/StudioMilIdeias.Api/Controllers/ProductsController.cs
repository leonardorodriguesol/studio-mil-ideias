using Microsoft.AspNetCore.Mvc;
using StudioMilIdeias.Application.UseCases.Products;

namespace StudioMilIdeias.Api.Controllers;

[ApiController]
[Route("products")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductQueries _productQueries;

    public ProductsController(IProductQueries productQueries)
    {
        _productQueries = productQueries;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await _productQueries.GetAllActiveAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await _productQueries.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        return Ok(item);
    }
}
