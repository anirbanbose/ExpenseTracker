
using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Domain.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ExpenseTracker.Infrastructure.Web.Auth;

public static class AuthServicesRegistration
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])),
                ClockSkew = TimeSpan.Zero
            };

            // Accept token from cookie
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = ctx =>
                {
                    if (ctx.Request.Cookies.ContainsKey(Constants.ACCESS_TOKEN_NAME))
                    {
                        ctx.Request.Cookies.TryGetValue(Constants.ACCESS_TOKEN_NAME, out var accessToken);
                        if (!string.IsNullOrWhiteSpace(accessToken))
                        {
                            ctx.Token = accessToken;
                        }
                    }
                    return Task.CompletedTask;
                }
            };
        });

        services.AddTransient<IAuthenticatedUserProvider, AuthenticatedUserProvider>();
        services.AddScoped<ITokenManager, TokenManager>();
        services.AddHttpContextAccessor();
        return services;
    }
}
