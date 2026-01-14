using Sportex.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;

public class TokenRefreshMiddleware
{
    private readonly RequestDelegate _next;

    public TokenRefreshMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IAuthService authService)
    {
        try
        {
            await _next(context);
        }
        catch (SecurityTokenExpiredException)
        {
            var refresh = context.Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refresh)) return;

            try
            {
                var tokens = await authService.RefreshTokenAsync(refresh);

                context.Response.Cookies.Append("access_token", tokens.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = false
                });

                context.Response.Cookies.Append("refresh_token", tokens.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = false
                });
            }
            catch
            {
                // swallow refresh failures — never crash request pipeline
            }
        }
    }
}
