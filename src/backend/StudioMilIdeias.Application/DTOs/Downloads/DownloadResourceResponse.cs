namespace StudioMilIdeias.Application.DTOs.Downloads;

public sealed class DownloadResourceResponse
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ResourceUrl { get; init; } = string.Empty;
}
