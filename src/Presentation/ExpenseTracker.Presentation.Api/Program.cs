using ExpenseTracker.Application;
using ExpenseTracker.Application.Contracts.Auth;
using ExpenseTracker.Domain.Utils;
using ExpenseTracker.Infrastructure.Email;
using ExpenseTracker.Infrastructure.Persistence;
using ExpenseTracker.Infrastructure.Report;
using ExpenseTracker.Presentation.Api;
using ExpenseTracker.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services
.AddApplicationServices(configuration)
.AddPersistenceServices(configuration)
.AddEmailServices(configuration)
.AddReportServices();


builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddAuthentication(options =>
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])),
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

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ICurrentUserManager, CurrenctUserManager>();
builder.Services.AddScoped<ITokenManager, TokenManager>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

await using var scope = app.Services.CreateAsyncScope();

var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
if (context is null)
    throw new Exception("Database Context Not Found");
await context.Database.MigrateAsync();

var seedService = scope.ServiceProvider.GetRequiredService<ISeedDataBase>();
await seedService.Seed();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
QuestPDF.Settings.License = LicenseType.Community;
app.Run();
