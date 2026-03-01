namespace StudioMilIdeias.Application.DTOs.Downloads;

public sealed class DownloadTokenResponse
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }
}
