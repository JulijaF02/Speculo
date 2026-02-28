using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Speculo.Identity.Controllers;

/// <summary>
/// User account endpoints — returns profile info from the JWT token.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
public class AccountController : ControllerBase
{
    /// <summary>Get the current authenticated user's profile.</summary>
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetMe()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var fullName = User.FindFirst(ClaimTypes.Name)?.Value;

        return Ok(new
        {
            Id = userId,
            Email = email,
            FullName = fullName
        });
    }
}
