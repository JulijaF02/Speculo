using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Speculo.Application.Common.Interfaces;
using Speculo.Domain.Entities;

namespace Speculo.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;

    // Constructor - DI nam ubrizgava JwtSettings preko Options Pattern-a
    public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    public string GenerateToken(User user)
    {
        // 1. Kreiramo "ključ" (Secret) od onog stringa iz User Secrets
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Secret)
        );

        // 2. Biramo algoritam potpisivanja (HMAC SHA-256 je industrijski standard)
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // 3. Pravimo "tvrdnje" (Claims) - podaci koji idu u token
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // Subject (User ID)
            new Claim(JwtRegisteredClaimNames.Email, user.Email),       // Email
            new Claim(JwtRegisteredClaimNames.Name, user.FullName),     // Puno ime
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Token ID (jedinstveni za svaki token)
        };

        // 4. Pravimo sam token sa svim informacijama
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,         // Ko ga izdaje
            audience: _jwtSettings.Audience,     // Za koga je namenjen
            claims: claims,                      // Šta nosi u sebi
            expires: DateTime.UtcNow.AddHours(2), // Koliko dugo važi (2 sata)
            signingCredentials: credentials      // Digitalni potpis
        );

        // 5. Pretvaramo ga u string koji možemo poslati klijentu
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
