using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Speculo.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        // Čitamo podatke direktno iz tokena (Claims)
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var fullName = User.FindFirst(ClaimTypes.Name)?.Value;

        return Ok(new 
        { 
            Message = "Token is valid!",
            Id = userId,
            Email = email,
            FullName = fullName
        });
    }
}