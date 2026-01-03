


//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;
//using Sportex.Application.DTOs.Auth;
//using Sportex.Application.Interfaces;
//using Sportex.Domain.Entities;
//using Sportex.Infrastructure.Data;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Security.Cryptography;
//using System.Text;

//namespace Sportex.Infrastructure.Services;

//public class AuthService : IAuthService
//{
//    private readonly SportexDbContext _context;
//    private readonly IConfiguration _config;

//    public AuthService(SportexDbContext context, IConfiguration config)
//    {
//        _context = context;
//        _config = config;
//    }

//    // ---------------- REGISTER ----------------
//    public async Task RegisterAsync(RegisterDto dto)
//    {
//        dto.Name = dto.Name.Trim();
//        dto.Email = dto.Email.Trim();
//        dto.Password = dto.Password.Trim();

//        if (string.IsNullOrWhiteSpace(dto.Name) ||
//            string.IsNullOrWhiteSpace(dto.Email) ||
//            string.IsNullOrWhiteSpace(dto.Password))
//            throw new Exception("Fields cannot be empty or spaces");

//        if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
//            throw new Exception("Email already exists");

//        var user = new User
//        {
//            Name = dto.Name,
//            Email = dto.Email,
//            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
//            Role = "user",
//            isBlocked = false,
//            CreatedOn = DateTime.UtcNow
//        };

//        _context.Users.Add(user);
//        await _context.SaveChangesAsync();
//    }

//    // ---------------- LOGIN ----------------
//    public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
//    {
//        dto.Email = dto.Email.Trim();
//        dto.Password = dto.Password.Trim();

//        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
//            throw new Exception("Invalid login");

//        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
//        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
//            throw new Exception("Invalid login");

//        var accessToken = GenerateJwtToken(user);
//        var refreshToken = GenerateRefreshToken();

//        user.RefreshToken = refreshToken;
//        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
//        await _context.SaveChangesAsync();

//        return new TokenResponseDto
//        {
//            AccessToken = accessToken,
//            RefreshToken = refreshToken
//        };
//    }

//    // ---------------- REFRESH TOKEN ----------------
//    public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
//    {
//        if (string.IsNullOrWhiteSpace(refreshToken))
//            throw new Exception("Invalid refresh token");

//        var user = await _context.Users.FirstOrDefaultAsync(x =>
//            x.RefreshToken == refreshToken &&
//            x.RefreshTokenExpiryTime > DateTime.UtcNow);

//        if (user == null) throw new Exception("Invalid refresh token");

//        var newAccess = GenerateJwtToken(user);
//        var newRefresh = GenerateRefreshToken();

//        user.RefreshToken = newRefresh;
//        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
//        await _context.SaveChangesAsync();

//        return new TokenResponseDto
//        {
//            AccessToken = newAccess,
//            RefreshToken = newRefresh
//        };
//    }

//    // ---------------- TOKEN HELPERS ----------------
//    private string GenerateJwtToken(User user)
//    {
//        var claims = new[]
//        {
//            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//            new Claim(ClaimTypes.Email, user.Email),
//            new Claim(ClaimTypes.Role, user.Role)   // 🔥 ROLE CLAIM ADDED (FIXES 403)
//        };

//        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
//        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//        var token = new JwtSecurityToken(
//            issuer: _config["Jwt:Issuer"],
//            audience: _config["Jwt:Audience"],
//            claims: claims,
//            expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpireMinutes"]!)),
//            signingCredentials: creds
//        );

//        return new JwtSecurityTokenHandler().WriteToken(token);
//    }

//    private string GenerateRefreshToken()
//    {
//        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
//    }
//}







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

    public AuthService(SportexDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // ---------------- REGISTER ----------------
    public async Task RegisterAsync(RegisterDto dto)
    {
        // 🔹 Empty check (DO NOT TRIM BEFORE VALIDATION)
        if (string.IsNullOrWhiteSpace(dto.Name) ||
            string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Password))
            throw new Exception("All fields are required");

        // 🔹 Name strict check (blocks leading/trailing spaces)
        if (!Regex.IsMatch(dto.Name, @"^[A-Za-z]{3,50}$"))
            throw new Exception("Name must contain only letters, no spaces, and at least 3 characters");

        // 🔹 Email strict format
        if (!Regex.IsMatch(dto.Email, @"^(?!example@)(?!test@)(?!demo@)(?!admin@)(?!user@)[a-z0-9]+([._%+-][a-z0-9]+)*@[a-z0-9-]+\.(com|in|net|org|co|edu)$"))
            throw new Exception("Enter a valid personal email address");

        // 🔹 Password strength
        if (dto.Password.Length < 6)
            throw new Exception("Password must be at least 6 characters");

        // 🔹 Now trim AFTER validation
        dto.Name = dto.Name.Trim();
        dto.Email = dto.Email.Trim().ToLower();
        dto.Password = dto.Password.Trim();

        // 🔹 Email uniqueness
        if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
            throw new Exception("Email already exists");

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "user",
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

        if (string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Password))
            throw new Exception("Invalid login credentials");

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

        // 🔹 Email / password check
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Invalid email or password");

        // 🔹 Blocked user check (IMPORTANT)
        if (user.isBlocked)
            throw new Exception("Your account is blocked");

        // 🔹 Generate tokens
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

    // ---------------- REFRESH TOKEN ----------------
    public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new Exception("Invalid refresh token");

        var user = await _context.Users.FirstOrDefaultAsync(x =>
            x.RefreshToken == refreshToken &&
            x.RefreshTokenExpiryTime > DateTime.UtcNow);

        if (user == null)
            throw new Exception("Refresh token expired or invalid");

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

    // ---------------- TOKEN HELPERS ----------------
    // ---------------- TOKEN HELPERS ----------------
    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
        new Claim("uid", user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        int minutes = int.Parse(_config["Jwt:ExpireMinutes"]!);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }


}
