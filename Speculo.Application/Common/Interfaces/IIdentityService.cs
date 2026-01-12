using Speculo.Application.Common.Models.Auth;

namespace Speculo.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
}