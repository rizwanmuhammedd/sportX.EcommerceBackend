




using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sportex.Application.Common;
using Sportex.Application.DTOs.Auth;
using Sportex.Application.DTOs.Users;
using Sportex.Application.Interfaces;

namespace Sportex.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;
    private readonly IUserService _userService;   // 🔹 ADDED

    public AuthController(IAuthService service, IUserService userService)  // 🔹 UPDATED
    {
        _service = service;
        _userService = userService;
    }

    // REGISTER
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.Fail(400, ModelState.Values
                .SelectMany(v => v.Errors)
                .First().ErrorMessage));

        await _service.RegisterAsync(dto);
        return Ok(ApiResponse.Success("Registered successfully"));
    }









    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.Fail(400, ModelState.Values
                .SelectMany(v => v.Errors)
                .First().ErrorMessage));

        try
        {
            var tokens = await _service.LoginAsync(dto);
            SetTokenCookies(tokens);

            return Ok(ApiResponse.Success("Login successful", new
            {
                accessToken = tokens.AccessToken,
                refreshToken = tokens.RefreshToken
            }));
        }
        catch (Exception ex)
        {
            // ✅ IMPORTANT — return 400 instead of 500
            return BadRequest(ApiResponse.Fail(400, ex.Message));
        }
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

    // ---------------- PROFILE ----------------

    [Authorize]
    [HttpGet("myProfile")]
    public async Task<IActionResult> MyProfile()
    {
        int userId = int.Parse(User.FindFirst("uid")!.Value);
        var data = await _userService.GetProfileAsync(userId);
        return Ok(ApiResponse.Success("Profile fetched", data));
    }

    [Authorize]
    [HttpPut("updateProfile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        int userId = int.Parse(User.FindFirst("uid")!.Value);
        await _userService.UpdateProfileAsync(userId, dto);
        return Ok(ApiResponse.Success("Profile updated"));
    }

    // ---------------- NEW FEATURES ----------------

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp(string email)
    {
        await _service.SendOtpAsync(email);
        return Ok(ApiResponse.Success("OTP sent to your email"));
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        await _service.ForgotPasswordAsync(email);
        return Ok(ApiResponse.Success("OTP sent for password reset"));
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        await _service.ResetPasswordAsync(dto);
        return Ok(ApiResponse.Success("Password reset successful"));
    }

    // COOKIE HELPER
    private void SetTokenCookies(TokenResponseDto tokens)
    {
        Response.Cookies.Append("access_token", tokens.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
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


    [Authorize]
    [HttpPatch("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        int userId = int.Parse(User.FindFirst("uid")!.Value);
        await _service.ChangePasswordAsync(userId, dto);
        return Ok(ApiResponse.Success("Password changed successfully"));
    }
    [Authorize]
    [HttpPost("upload-avatar")]
    public async Task<IActionResult> UploadAvatar([FromForm] UploadAvatarDto dto)
    {
        int userId = int.Parse(User.FindFirst("uid")!.Value);

        if (dto.Image == null || dto.Image.Length == 0)
            return BadRequest("No image uploaded");

        var fileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
        var path = Path.Combine("wwwroot/avatars", fileName);

        using var stream = new FileStream(path, FileMode.Create);
        await dto.Image.CopyToAsync(stream);

        var url = $"{Request.Scheme}://{Request.Host}/avatars/{fileName}";

        await _userService.UpdateAvatarAsync(userId, url);

        return Ok(ApiResponse.Success(url));
    }






}
