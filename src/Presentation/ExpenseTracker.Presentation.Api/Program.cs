using ExpenseTracker.Application;
using ExpenseTracker.Infrastructure.Email;
using ExpenseTracker.Infrastructure.Persistence;
using ExpenseTracker.Infrastructure.Report;
using ExpenseTracker.Presentation.Api.Middlewares;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using Serilog;
using ExpenseTracker.Infrastructure.Web.Auth;

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
.AddReportServices();


//builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

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
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
QuestPDF.Settings.License = LicenseType.Community;
app.Run();
