using ExpenseTracker.Application;
using ExpenseTracker.Infrastructure.BackgroundJobs;
using ExpenseTracker.Infrastructure.Email;
using ExpenseTracker.Infrastructure.Persistence;
using ExpenseTracker.Infrastructure.Report;
using ExpenseTracker.Infrastructure.Web.Auth;
using ExpenseTracker.Presentation.Api.Middlewares;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using Serilog;

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
.AddAuthServices(configuration)
.AddBackgroundJobServices(configuration)
.AddReportServices();


// Add CORS policy
var frontEndAddr = configuration["FrontEndUrl"];
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontEndClient",
        policy =>
        {
            policy.WithOrigins(frontEndAddr!) 
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); ;
        });
});


Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Host.UseSerilog();


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

app.UseCors("AllowFrontEndClient");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseHangfireDashboard("/hangfire-jobs");
app.MapHangfireDashboard();


QuestPDF.Settings.License = LicenseType.Community;
app.Run();
