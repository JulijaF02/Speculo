using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Speculo.Application.Common.Interfaces;
using Speculo.Application.Common.Models.Auth;
using Speculo.Domain.Entities;

namespace Speculo.Infrastructure.Services;

public class IdentityService(ISpeculoDbContext context, IJwtTokenGenerator tokenGenerator) : IIdentityService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var existingUser = await context.Users.AnyAsync(u => u.Email == request.Email, ct);
        if (existingUser)
            throw new Exception("User with this email already exists.");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = hashedPassword,
            FullName = request.FullName,
            RegisteredAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(ct);

        var token = tokenGenerator.GenerateToken(user);

        return new AuthResponse(user.Id, user.Email, user.FullName, token);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, ct);
        if (user is null)
            throw new Exception("Invalid email or password");

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
            throw new Exception("Invalid email or password");

        var token = tokenGenerator.GenerateToken(user);

        return new AuthResponse(user.Id, user.Email, user.FullName, token);
    }
}
