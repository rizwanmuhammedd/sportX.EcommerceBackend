using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using sportex.Infrastructure.Services;
using Sportex.Infrastructure;
using Sportex.Infrastructure.Data;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. SERVICES CONFIGURATION
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 🔥 CORS CONFIGURATION - Setup for single origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
               
            "http://localhost:5174",
            "https://localhost:5174",
            "https://api.razorpay.com"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});


// SWAGGER CONFIGURATION
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sportex API",
        Version = "v1",
        Description = "Sportex Ecommerce Backend"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter ONLY your access token (no 'Bearer ' word)",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});





// Dependency Injection & Auth Services
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(opt =>
{
    //opt.TokenValidationParameters = new TokenValidationParameters
    //{
    //    ValidateIssuer = true,
    //    ValidateAudience = true,
    //    ValidateLifetime = true,
    //    ValidateIssuerSigningKey = true,
    //    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    //    ValidAudience = builder.Configuration["Jwt:Audience"],
    //    IssuerSigningKey = new SymmetricSecurityKey(
    //        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
    //    ClockSkew = TimeSpan.Zero,

    //    // 🔥🔥 THIS LINE FIXES EVERYTHING
    //    RoleClaimType = ClaimTypes.Role
    //};





    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ClockSkew = TimeSpan.Zero,

        // 🔥 FIX ROLE & USER ID MAPPING
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
    };


    //opt.Events = new JwtBearerEvents
    //{
    //    OnMessageReceived = context =>
    //    {
    //        var token = context.Request.Cookies["access_token"];
    //        if (!string.IsNullOrEmpty(token))
    //        {
    //            context.Token = token;
    //        }
    //        return Task.CompletedTask;
    //    }
    //};




    opt.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // 1) Try header first (Swagger / Postman / React)
            var authHeader = context.Request.Headers["Authorization"]
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                context.Token = authHeader.Substring("Bearer ".Length);
                return Task.CompletedTask;
            }

            // 2) Fallback to cookie (your existing behavior)
            var cookieToken = context.Request.Cookies["access_token"];
            if (!string.IsNullOrEmpty(cookieToken))
            {
                context.Token = cookieToken;
            }

            return Task.CompletedTask;
        }
    };
    
});


builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<CloudinaryService>();

var app = builder.Build();

// 2. MIDDLEWARE PIPELINE (The Order is critical here)

// Fixes the 404 error
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sportex API v1");
    });
}


app.UseHttpsRedirection();

// 🔥 CORS MUST come before Authentication/Authorization
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseMiddleware<TokenRefreshMiddleware>();
app.UseAuthorization();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "avatars")),
    RequestPath = "/avatars"
});

app.MapControllers();

// 3. DATABASE SEEDING
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SportexDbContext>();
    AdminSeeder.SeedAdmin(context);
}

app.Run();