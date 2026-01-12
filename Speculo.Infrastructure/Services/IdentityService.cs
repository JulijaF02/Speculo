using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Speculo.Application.Common.Interfaces;
using Speculo.Application.Common.Models.Auth;
using Speculo.Domain.Entities;

namespace Speculo.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly ISpeculoDbContext _context;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public IdentityService(ISpeculoDbContext context, IJwtTokenGenerator tokenGenerator)
    {
        _context = context;
        _tokenGenerator = tokenGenerator;
    }
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        // 1. Provera da li korisnik već postoji
        var existingUser = await _context.Users.AnyAsync(u => u.Email == request.Email, ct);
        if (existingUser) throw new Exception("User with this email already exists.");

        // 2. (Hashing)
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // 3. Pravljenje Entiteta za bazu
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = hashedPassword,
            FullName = request.FullName,
            RegisteredAt = DateTime.UtcNow
        };

        // 4. Upis u bazu
        _context.Users.Add(user);
        await _context.SaveChangesAsync(ct);
        var token = _tokenGenerator.GenerateToken(user);
        // Vraćamo odgovor (Token je za sada prazan, to je sledeća lekcija!)
        return new AuthResponse(user.Id, user.Email, user.FullName, token);
    }


    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, ct);
        if(user==null) 
            throw new Exception("Invalid email or password");
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if(!isPasswordValid)
            throw new Exception("Invalid email or password");
         var token = _tokenGenerator.GenerateToken(user);
        return new AuthResponse(user.Id, user.Email, user.FullName, token);

    }
}