namespace StudioMilIdeias.Application.Abstractions;

public sealed class DigitalDownloadTokenPayload
{
    public Guid UserId { get; init; }
    public Guid OrderId { get; init; }
    public Guid ProductId { get; init; }
    public DateTime ExpiresAtUtc { get; init; }
}

public sealed class DigitalDownloadTokenIssueResult
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }
}

public interface IDigitalDownloadTokenService
{
    DigitalDownloadTokenIssueResult Issue(Guid userId, Guid orderId, Guid productId);
    DigitalDownloadTokenPayload? Read(string token);
}
