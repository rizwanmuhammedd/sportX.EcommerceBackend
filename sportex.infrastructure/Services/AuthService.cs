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
//             Role = "user",
//            isBlocked=false,
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
//            new Claim(ClaimTypes.Email, user.Email)
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
        dto.Name = dto.Name.Trim();
        dto.Email = dto.Email.Trim();
        dto.Password = dto.Password.Trim();

        if (string.IsNullOrWhiteSpace(dto.Name) ||
            string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Password))
            throw new Exception("Fields cannot be empty or spaces");

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
        dto.Email = dto.Email.Trim();
        dto.Password = dto.Password.Trim();

        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            throw new Exception("Invalid login");

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Invalid login");

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

        if (user == null) throw new Exception("Invalid refresh token");

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
    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)   // 🔥 ROLE CLAIM ADDED (FIXES 403)
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
}
