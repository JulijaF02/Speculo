namespace Speculo.Application.Common.Models.Auth;

public record RegisterRequest(
    string Email,
    string Password,
    string FullName
);