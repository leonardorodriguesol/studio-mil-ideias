using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudioMilIdeias.Application.UseCases.Downloads;
using StudioMilIdeias.Application.UseCases.Orders;

namespace StudioMilIdeias.Api.Controllers;

[ApiController]
[Authorize]
[Route("orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderQueries _orderQueries;
    private readonly IDigitalDeliveryService _digitalDeliveryService;

    public OrdersController(
        IOrderQueries orderQueries,
        IDigitalDeliveryService digitalDeliveryService)
    {
        _orderQueries = orderQueries;
        _digitalDeliveryService = digitalDeliveryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var orders = await _orderQueries.GetByUserIdAsync(userId, cancellationToken);
            return Ok(orders);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var order = await _orderQueries.GetByIdAndUserIdAsync(id, userId, cancellationToken);
            if (order is null)
            {
                return NotFound();
            }

            return Ok(order);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpGet("{orderId:guid}/items/{productId:guid}/download-link")]
    public async Task<IActionResult> GetDownloadLink(
        Guid orderId,
        Guid productId,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var tokenResponse = await _digitalDeliveryService.GenerateDownloadTokenAsync(
                userId,
                orderId,
                productId,
                cancellationToken);

            var url = Url.ActionLink(
                nameof(ResolveDownload),
                values: new { token = tokenResponse.Token })
                ?? $"/downloads/{tokenResponse.Token}";

            return Ok(new
            {
                downloadUrl = url,
                expiresAtUtc = tokenResponse.ExpiresAtUtc
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpGet("/downloads/{token}")]
    public async Task<IActionResult> ResolveDownload(string token, CancellationToken cancellationToken)
    {
        try
        {
            var resource = await _digitalDeliveryService.ResolveDownloadAsync(token, cancellationToken);
            return Ok(resource);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    private Guid GetCurrentUserId()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(claimValue, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user token.");
        }

        return userId;
    }
}
