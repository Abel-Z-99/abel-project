using Application.Auth;
using Application.Dashboard;
using Application.Orders;
using Application.Products;
using Application.Users;
using Infrastructure.Auth;
using Infrastructure.Dashboard;
using Infrastructure.Data;
using Infrastructure.Orders;
using Infrastructure.Products;
using Infrastructure.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IUserAdminService, UserAdminService>();
        services.AddScoped<IDashboardService, DashboardService>();

        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()!;
        var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            })
            .AddJwtBearer("Bearer", options =>
            {
                // 不把 JWT 声明映射成 .NET 长 URI，否则控制器里 User.FindFirst("sub") / "email" 会拿不到
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    NameClaimType = JwtRegisteredClaimNames.UniqueName,
                    RoleClaimType = ClaimTypes.Role
                };
            });

        services.AddAuthorization();

        return services;
    }
}
