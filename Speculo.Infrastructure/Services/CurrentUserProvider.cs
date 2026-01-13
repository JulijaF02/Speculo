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
            //getting the user object from the current request
            var user = httpContextAccessor.HttpContext?.User;

            //look for a name identifier or a sub claim
            var idClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user?.FindFirst("sub")?.Value;

            //try to turn that text into an guid id
            if (Guid.TryParse(idClaim, out var guid))
            {
                return guid;
            }
            return null;

        }
    }
}