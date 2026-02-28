using System.Text.Json;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Speculo.Contracts.Events;
using Speculo.Identity.Data;
using Speculo.Identity.Models;

namespace Speculo.Identity.Services;

/// <summary>
/// Handles user registration and login.
/// On registration, publishes a UserRegisteredEvent to Kafka
/// so other services (e.g., Tracking) can create their own user record if needed.
/// </summary>
public class AuthService(
    IdentityDbContext context,
    JwtTokenService tokenService,
    IProducer<string, string> kafkaProducer,
    ILogger<AuthService> logger)
{
    private const string Topic = "speculo-events";

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        // Check if user already exists
        var exists = await context.Users.AnyAsync(u => u.Email == request.Email, ct);
        if (exists)
            throw new InvalidOperationException("User with this email already exists.");

        // Hash password and create user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName,
            RegisteredAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(ct);

        // Publish UserRegisteredEvent to Kafka — other services need to know a user was created
        var integrationEvent = new UserRegisteredEvent(
            UserId: user.Id,
            Email: user.Email,
            RegisteredAt: user.RegisteredAt
        );

        try
        {
            var message = new Message<string, string>
            {
                Key = integrationEvent.EventType,
                Value = JsonSerializer.Serialize(integrationEvent)
            };

            await kafkaProducer.ProduceAsync(Topic, message, ct);
            logger.LogInformation("Published UserRegisteredEvent for {UserId}", user.Id);
        }
        catch (ProduceException<string, string> ex)
        {
            // Log but don't fail the registration — user is already saved
            logger.LogError(ex, "Failed to publish UserRegisteredEvent for {UserId}", user.Id);
        }

        var token = tokenService.GenerateToken(user);
        return new AuthResponse(user.Id, user.Email, user.FullName, token);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, ct);
        if (user is null)
            throw new InvalidOperationException("Invalid email or password");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new InvalidOperationException("Invalid email or password");

        var token = tokenService.GenerateToken(user);
        return new AuthResponse(user.Id, user.Email, user.FullName, token);
    }
}
