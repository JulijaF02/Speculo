namespace Speculo.Identity.Models;

public record RegisterRequest(string Email, string Password, string FullName);
public record LoginRequest(string Email, string Password);
public record AuthResponse(Guid Id, string Email, string FullName, string Token);
