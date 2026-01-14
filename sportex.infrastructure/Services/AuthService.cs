using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Sportex.Application.DTOs.Auth;
using Sportex.Application.Interfaces;
using Sportex.Domain.Entities;
using Sportex.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Sportex.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly SportexDbContext _context;
    private readonly IConfiguration _config;
    private readonly EmailService _emailService;

    public AuthService(SportexDbContext context, IConfiguration config, EmailService emailService)
    {
        _context = context;
        _config = config;
        _emailService = emailService;
    }

    // ---------------- REGISTER ----------------
    public async Task RegisterAsync(RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) ||
            string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Password))
            throw new Exception("All fields are required");

        if (!Regex.IsMatch(dto.Name, @"^[A-Za-z]{3,50}$"))
            throw new Exception("Name must contain only letters");

        if (dto.Password.Length < 6)
            throw new Exception("Password must be at least 6 characters");

        dto.Name = dto.Name.Trim();
        dto.Email = dto.Email.Trim().ToLower();
        dto.Password = dto.Password.Trim();

        if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
            throw new Exception("Email already exists");

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "user",                 // 🔥 FIXED
            isBlocked = false,
            CreatedOn = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    // ---------------- LOGIN ----------------
    public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
    {
        dto.Email = dto.Email.Trim().ToLower();
        dto.Password = dto.Password.Trim();

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Invalid email or password");

        if (user.isBlocked)
            throw new Exception("Your account is blocked");

        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _context.SaveChangesAsync();

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    // ---------------- REFRESH ----------------
    public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x =>
            x.RefreshToken == refreshToken &&
            x.RefreshTokenExpiryTime > DateTime.UtcNow);

        if (user == null)
            throw new Exception("Invalid refresh token");

        var newAccess = GenerateJwtToken(user);
        var newRefresh = GenerateRefreshToken();

        user.RefreshToken = newRefresh;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _context.SaveChangesAsync();

        return new TokenResponseDto
        {
            AccessToken = newAccess,
            RefreshToken = newRefresh
        };
    }

    // ---------------- TOKEN ----------------
    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim("uid", user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)     // 🔥 FIXED
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpireMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    // ---------------- OTP ----------------
    public async Task SendOtpAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email.ToLower());
        if (user == null) throw new Exception("User not found");

        user.Otp = new Random().Next(100000, 999999).ToString();
        user.OtpExpiry = DateTime.UtcNow.AddMinutes(5);

        await _context.SaveChangesAsync();
        _emailService.Send(user.Email, "OTP", $"Your OTP is {user.Otp}");
    }

    public async Task ForgotPasswordAsync(string email) => await SendOtpAsync(email);

    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email.ToLower());

        if (user == null || user.Otp != dto.Otp || user.OtpExpiry < DateTime.UtcNow)
            throw new Exception("Invalid OTP");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.Otp = null;
        user.OtpExpiry = null;

        await _context.SaveChangesAsync();
    }

    // ---------------- REGISTER + AUTO LOGIN ----------------
    public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _context.Users.FindAsync(userId);

        if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user!.PasswordHash))
            throw new Exception("Old password is incorrect");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _context.SaveChangesAsync();
    }


}
