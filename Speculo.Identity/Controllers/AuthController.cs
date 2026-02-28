using Microsoft.AspNetCore.Mvc;
using Speculo.Identity.Models;
using Speculo.Identity.Services;
using Speculo.Identity.Validation;

namespace Speculo.Identity.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken ct)
    {
        var errors = AuthValidation.ValidateRegister(request.Email, request.Password, request.FullName);
        if (errors.Count > 0)
        {
            return BadRequest(new { errors });
        }

        var result = await authService.RegisterAsync(request, ct);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var errors = AuthValidation.ValidateLogin(request.Email, request.Password);
        if (errors.Count > 0)
        {
            return BadRequest(new { errors });
        }

        var result = await authService.LoginAsync(request, ct);
        return Ok(result);
    }
}
