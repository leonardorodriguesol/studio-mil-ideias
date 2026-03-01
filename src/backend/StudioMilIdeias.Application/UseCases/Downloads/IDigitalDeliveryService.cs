using StudioMilIdeias.Application.DTOs.Downloads;

namespace StudioMilIdeias.Application.UseCases.Downloads;

public interface IDigitalDeliveryService
{
    Task<DownloadTokenResponse> GenerateDownloadTokenAsync(
        Guid userId,
        Guid orderId,
        Guid productId,
        CancellationToken cancellationToken = default);

    Task<DownloadResourceResponse> ResolveDownloadAsync(
        string token,
        CancellationToken cancellationToken = default);
}
