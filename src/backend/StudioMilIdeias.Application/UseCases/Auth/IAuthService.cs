using StudioMilIdeias.Application.DTOs.Auth;

namespace StudioMilIdeias.Application.UseCases.Auth;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
