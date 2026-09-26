FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Restore first for better layer caching
COPY Mandys.Api/Mandys.Api.csproj Mandys.Api/
COPY Mandys.Application/Mandys.Application.csproj Mandys.Application/
COPY Mandys.Infrastructure/Mandys.Infrastructure.csproj Mandys.Infrastructure/
COPY Mandys.Domain/Mandys.Domain.csproj Mandys.Domain/
COPY Directory.Build.props ./
RUN dotnet restore Mandys.Api/Mandys.Api.csproj

# Copy the rest of the source then publish
COPY . .

RUN dotnet publish Mandys.Api/Mandys.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Single runtime image for both flows:
# - dev (docker/compose*.yml) sets ASPNETCORE_ENVIRONMENT=Development, which
#   enables Scalar/OpenAPI plus startup auto-migration;
# - prod (GHCR image) runs with the default Production environment.
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
USER app
ENTRYPOINT ["dotnet", "Mandys.Api.dll"]
