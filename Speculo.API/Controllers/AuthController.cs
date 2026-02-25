using Microsoft.AspNetCore.Mvc;
using Speculo.Application.Common.Interfaces;
using Speculo.Application.Common.Models.Auth;
using Microsoft.AspNetCore.RateLimiting;
namespace Speculo.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("auth")]

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

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var result = await _identityService.LoginAsync(request);
        return Ok(result);
    }
}