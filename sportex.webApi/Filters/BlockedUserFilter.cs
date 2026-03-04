using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Sportex.Infrastructure.Data;
using System.Security.Claims;

namespace Sportex.WebApi.Filters
{
    public class BlockedUserFilter : IAsyncAuthorizationFilter
    {
        private readonly SportexDbContext _context;

        public BlockedUserFilter(SportexDbContext context)
        {
            _context = context;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // If endpoint allows anonymous access, skip
            if (context.ActionDescriptor.EndpointMetadata
                .Any(em => em is Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute))
                return;

            var userIdClaim = context.HttpContext.User.FindFirst("uid");
            if (userIdClaim == null) return;

            int userId = int.Parse(userIdClaim.Value);

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.isBlocked)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    message = "User is blocked"
                });
            }
        }
    }
}
