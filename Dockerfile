# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/UserService.API/UserService.API.csproj", "UserService.API/"]
COPY ["src/UserService.Application/UserService.Application.csproj", "UserService.Application/"]
COPY ["src/UserService.Domain/UserService.Domain.csproj", "UserService.Domain/"]
COPY ["src/UserService.Infrastructure/UserService.Infrastructure.csproj", "UserService.Infrastructure/"]

RUN dotnet restore "UserService.API/UserService.API.csproj"

# Copy all source files
COPY src/ .

# Build the application
WORKDIR "/src/UserService.API"
RUN dotnet build "UserService.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "UserService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.API.dll"]
