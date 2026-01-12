namespace Speculo.Application.Common.Models.Auth;

public record LoginRequest(
    string Email,
    string Password
);