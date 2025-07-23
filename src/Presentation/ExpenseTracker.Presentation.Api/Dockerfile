# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Presentation/ExpenseTracker.Presentation.Api/ExpenseTracker.Presentation.Api.csproj", "src/Presentation/ExpenseTracker.Presentation.Api/"]
COPY ["src/Core/ExpenseTracker.Application/ExpenseTracker.Application.csproj", "src/Core/ExpenseTracker.Application/"]
COPY ["src/Core/ExpenseTracker.Domain/ExpenseTracker.Domain.csproj", "src/Core/ExpenseTracker.Domain/"]
COPY ["src/Infrastructure/ExpenseTracker.Infrastructure.BackgroundJobs/ExpenseTracker.Infrastructure.BackgroundJobs.csproj", "src/Infrastructure/ExpenseTracker.Infrastructure.BackgroundJobs/"]
COPY ["src/Infrastructure/ExpenseTracker.Infrastructure.Web.Auth/ExpenseTracker.Infrastructure.Web.Auth.csproj", "src/Infrastructure/ExpenseTracker.Infrastructure.Web.Auth/"]
COPY ["src/Infrastructure/ExpenseTracker.Infrastructure.Persistence/ExpenseTracker.Infrastructure.Persistence.csproj", "src/Infrastructure/ExpenseTracker.Infrastructure.Persistence/"]
COPY ["src/Infrastructure/ExpenseTracker.Infrastructure.Email/ExpenseTracker.Infrastructure.Email.csproj", "src/Infrastructure/ExpenseTracker.Infrastructure.Email/"]
COPY ["src/Infrastructure/ExpenseTracker.Infrastructure.Report/ExpenseTracker.Infrastructure.Report.csproj", "src/Infrastructure/ExpenseTracker.Infrastructure.Report/"]
RUN dotnet restore "./src/Presentation/ExpenseTracker.Presentation.Api/ExpenseTracker.Presentation.Api.csproj"
COPY . .
WORKDIR "/src/src/Presentation/ExpenseTracker.Presentation.Api"
RUN dotnet build "./ExpenseTracker.Presentation.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ExpenseTracker.Presentation.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExpenseTracker.Presentation.Api.dll"]