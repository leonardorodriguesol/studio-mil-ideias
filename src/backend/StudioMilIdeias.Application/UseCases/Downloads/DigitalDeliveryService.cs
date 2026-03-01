using StudioMilIdeias.Application.Abstractions;
using StudioMilIdeias.Application.DTOs.Downloads;

namespace StudioMilIdeias.Application.UseCases.Downloads;

public sealed class DigitalDeliveryService : IDigitalDeliveryService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDigitalDownloadTokenService _digitalDownloadTokenService;

    public DigitalDeliveryService(
        IOrderRepository orderRepository,
        IDigitalDownloadTokenService digitalDownloadTokenService)
    {
        _orderRepository = orderRepository;
        _digitalDownloadTokenService = digitalDownloadTokenService;
    }

    public async Task<DownloadTokenResponse> GenerateDownloadTokenAsync(
        Guid userId,
        Guid orderId,
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var access = await _orderRepository.GetPaidOrderItemAccessAsync(
            userId,
            orderId,
            productId,
            cancellationToken);

        if (access is null)
        {
            throw new KeyNotFoundException("Paid order item not found.");
        }

        var issued = _digitalDownloadTokenService.Issue(userId, orderId, productId);
        return new DownloadTokenResponse
        {
            Token = issued.Token,
            ExpiresAtUtc = issued.ExpiresAtUtc
        };
    }

    public async Task<DownloadResourceResponse> ResolveDownloadAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        var payload = _digitalDownloadTokenService.Read(token);
        if (payload is null)
        {
            throw new UnauthorizedAccessException("Invalid or expired download token.");
        }

        var access = await _orderRepository.GetPaidOrderItemAccessAsync(
            payload.UserId,
            payload.OrderId,
            payload.ProductId,
            cancellationToken);

        if (access is null)
        {
            throw new KeyNotFoundException("Download resource not found.");
        }

        return new DownloadResourceResponse
        {
            ProductId = access.ProductId,
            ProductName = access.ProductName,
            ResourceUrl = access.DigitalFileUrl
        };
    }
}
