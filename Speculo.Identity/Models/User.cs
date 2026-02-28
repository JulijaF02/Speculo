namespace Speculo.Identity.Models;

/// <summary>
/// User entity for the Identity Service.
/// This is separate from Speculo.Domain.Entities.User — each service owns its own model.
/// The Identity Service only cares about authentication fields.
/// </summary>
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
}
