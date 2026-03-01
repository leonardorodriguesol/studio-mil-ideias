namespace StudioMilIdeias.Application.DTOs.Auth;

public sealed class AuthResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}
