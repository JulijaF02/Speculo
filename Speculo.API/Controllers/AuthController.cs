using Microsoft.AspNetCore.Mvc;
using Speculo.Application.Common.Interfaces;
using Speculo.Application.Common.Models.Auth;

namespace Speculo.API.Controllers;

[ApiController]
[Route("api/[controller]")] 
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var result = await _identityService.RegisterAsync(request);
        return Ok(result);
    }
}