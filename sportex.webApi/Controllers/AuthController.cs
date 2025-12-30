using Microsoft.AspNetCore.Mvc;
using Sportex.Application.DTOs.Auth;
using Sportex.Application.Interfaces;

namespace Sportex.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;
    public AuthController(IAuthService service) => _service = service;

    // REGISTER
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        await _service.RegisterAsync(dto);
        return Ok("Registered Successfully");
    }

    // LOGIN (returns Access + Refresh token)
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var tokens = await _service.LoginAsync(dto);
        return Ok(tokens);
    }

    // REFRESH TOKEN
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenDto dto)
    {
        var tokens = await _service.RefreshTokenAsync(dto.RefreshToken);
        return Ok(tokens);
    }
}
