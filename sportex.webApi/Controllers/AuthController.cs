//using Microsoft.AspNetCore.Mvc;
//using Sportex.Application.Common;
//using Sportex.Application.DTOs.Auth;
//using Sportex.Application.Interfaces;

//namespace Sportex.WebApi.Controllers;

//[ApiController]
//[Route("api/auth")]
//public class AuthController : ControllerBase
//{
//    private readonly IAuthService _service;
//    public AuthController(IAuthService service) => _service = service;

//    // REGISTER (VALIDATION ENABLED)
//    [HttpPost("register")]
//    public async Task<IActionResult> Register(RegisterDto dto)
//    {
//        if (!ModelState.IsValid)
//            return BadRequest(ApiResponse.Fail(400, "Invalid input"));

//        await _service.RegisterAsync(dto);
//        return Ok(ApiResponse.Success("Registered successfully"));
//    }


//    // LOGIN (COOKIE BASED)
//    [HttpPost("login")]
//    public async Task<IActionResult> Login(LoginDto dto)
//    {
//        var tokens = await _service.LoginAsync(dto);
//        SetTokenCookies(tokens);
//        return Ok("Login successful");
//    }

//    // REFRESH COOKIE TOKEN
//    [HttpPost("refresh")]
//    public async Task<IActionResult> Refresh()
//    {
//        var refreshToken = Request.Cookies["refresh_token"];
//        if (refreshToken == null) return Unauthorized("No refresh token");

//        var tokens = await _service.RefreshTokenAsync(refreshToken);
//        SetTokenCookies(tokens);
//        return Ok("Token refreshed");
//    }

//    // LOGOUT
//    [HttpPost("logout")]
//    public IActionResult Logout()
//    {
//        Response.Cookies.Delete("access_token");
//        Response.Cookies.Delete("refresh_token");
//        return Ok("Logged out");
//    }

//    // COOKIE HELPER
//    private void SetTokenCookies(TokenResponseDto tokens)
//    {
//        Response.Cookies.Append("access_token", tokens.AccessToken, new CookieOptions
//        {
//            HttpOnly = true,
//            Secure = false,              // localhost support
//            SameSite = SameSiteMode.Lax,
//            Expires = DateTime.UtcNow.AddMinutes(15)
//        });

//        Response.Cookies.Append("refresh_token", tokens.RefreshToken, new CookieOptions
//        {
//            HttpOnly = true,
//            Secure = false,
//            SameSite = SameSiteMode.Lax,
//            Expires = DateTime.UtcNow.AddDays(7)
//        });
//    }
//}







using Microsoft.AspNetCore.Mvc;
using Sportex.Application.Common;
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
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.Fail(400, "Invalid input"));

        await _service.RegisterAsync(dto);
        return Ok(ApiResponse.Success("Registered successfully"));
    }

    // LOGIN (COOKIE BASED)
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.Fail(400, "Invalid login data"));

        var tokens = await _service.LoginAsync(dto);
        SetTokenCookies(tokens);

        return Ok(ApiResponse.Success("Login successful", new
        {
            accessToken = tokens.AccessToken,
            refreshToken = tokens.RefreshToken
        }));
    }


    // REFRESH TOKEN
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        if (refreshToken == null)
            return Unauthorized(ApiResponse.Fail(401, "No refresh token found"));

        var tokens = await _service.RefreshTokenAsync(refreshToken);
        SetTokenCookies(tokens);

        return Ok(ApiResponse.Success("Token refreshed", new
        {
            accessToken = tokens.AccessToken,
            refreshToken = tokens.RefreshToken
        }));

    }

    // LOGOUT
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");
        return Ok(ApiResponse.Success("Logged out successfully"));
    }

    // COOKIE HELPER
    private void SetTokenCookies(TokenResponseDto tokens)
    {
        Response.Cookies.Append("access_token", tokens.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,     // localhost
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddMinutes(15)
        });

        Response.Cookies.Append("refresh_token", tokens.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(7)
        });
    }
}
