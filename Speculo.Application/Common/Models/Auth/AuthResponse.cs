namespace Speculo.Application.Common.Models.Auth;

public record AuthResponse(
    Guid Id,
    string Email,
    string FullName,
    string Token
);