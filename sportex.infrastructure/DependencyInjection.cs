using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Sportex.Application.Interfaces;                // <-- Interfaces live here
using Sportex.Infrastructure.Data;
using Sportex.Infrastructure.Repositories;           // <-- Implementations live here
using Sportex.Infrastructure.Services;

namespace Sportex.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<SportexDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        // ================== REPOSITORIES ==================
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // ================== SERVICES ==================
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IWishlistService, WishlistService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPaymentService, PaymentService>();


        // ================== UTILITIES ==================
        services.AddScoped<EmailService>();

        return services;
    }
}
