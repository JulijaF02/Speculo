using Speculo.Domain.Entities;

namespace Speculo.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}