using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using StudioMilIdeias.Application.Abstractions;

namespace StudioMilIdeias.Infrastructure.Payments;

public sealed class DigitalDownloadTokenService : IDigitalDownloadTokenService
{
    private readonly DigitalDeliveryOptions _options;

    public DigitalDownloadTokenService(IOptions<DigitalDeliveryOptions> options)
    {
        _options = options.Value;
    }

    public DigitalDownloadTokenIssueResult Issue(Guid userId, Guid orderId, Guid productId)
    {
        var secretBytes = GetSecretBytes();
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(GetTokenTtlMinutes());

        var payload = new TokenPayloadJson
        {
            UserId = userId,
            OrderId = orderId,
            ProductId = productId,
            ExpiresAtUtc = expiresAtUtc
        };

        var payloadJson = JsonSerializer.Serialize(payload);
        var payloadBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));
        var signatureBase64 = Base64UrlEncode(ComputeSignature(payloadBase64, secretBytes));

        return new DigitalDownloadTokenIssueResult
        {
            Token = $"{payloadBase64}.{signatureBase64}",
            ExpiresAtUtc = expiresAtUtc
        };
    }

    public DigitalDownloadTokenPayload? Read(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        try
        {
            var secretBytes = GetSecretBytes();
            var parts = token.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                return null;
            }

            var payloadBase64 = parts[0];
            var signatureBase64 = parts[1];
            var expectedSignature = ComputeSignature(payloadBase64, secretBytes);
            var receivedSignature = Base64UrlDecode(signatureBase64);
            if (!CryptographicOperations.FixedTimeEquals(expectedSignature, receivedSignature))
            {
                return null;
            }

            var payloadBytes = Base64UrlDecode(payloadBase64);
            var payload = JsonSerializer.Deserialize<TokenPayloadJson>(payloadBytes);
            if (payload is null || payload.ExpiresAtUtc <= DateTime.UtcNow)
            {
                return null;
            }

            return new DigitalDownloadTokenPayload
            {
                UserId = payload.UserId,
                OrderId = payload.OrderId,
                ProductId = payload.ProductId,
                ExpiresAtUtc = payload.ExpiresAtUtc
            };
        }
        catch (Exception ex) when (ex is FormatException or JsonException or CryptographicException)
        {
            return null;
        }
    }

    private byte[] GetSecretBytes()
    {
        if (string.IsNullOrWhiteSpace(_options.TokenSecret))
        {
            throw new InvalidOperationException("DigitalDelivery:TokenSecret configuration is required.");
        }

        return Encoding.UTF8.GetBytes(_options.TokenSecret);
    }

    private int GetTokenTtlMinutes()
    {
        return _options.TokenTtlMinutes > 0 ? _options.TokenTtlMinutes : 15;
    }

    private static byte[] ComputeSignature(string payloadBase64, byte[] secretBytes)
    {
        using var hmac = new HMACSHA256(secretBytes);
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(payloadBase64));
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string input)
    {
        var normalized = input
            .Replace('-', '+')
            .Replace('_', '/');

        return (normalized.Length % 4) switch
        {
            2 => Convert.FromBase64String($"{normalized}=="),
            3 => Convert.FromBase64String($"{normalized}="),
            _ => Convert.FromBase64String(normalized)
        };
    }

    private sealed class TokenPayloadJson
    {
        public Guid UserId { get; init; }
        public Guid OrderId { get; init; }
        public Guid ProductId { get; init; }
        public DateTime ExpiresAtUtc { get; init; }
    }
}
