using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Speculo.Application.Common.Interfaces;
namespace Speculo.Infrastructure.Services;

public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    public Guid? UserId
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;

            var idClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? user?.FindFirst("sub")?.Value;

            if (Guid.TryParse(idClaim, out var guid))
            {
                return guid;
            }

            return null;
        }
    }
}