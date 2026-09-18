FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS development
WORKDIR /src

RUN dotnet tool install -g dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV DOTNET_WATCH_RESTART_ON_RUDE_EDIT=true

EXPOSE 8080

# Compose mounts the repo at /src so `dotnet watch` hot-reloads on edit.
ENTRYPOINT ["dotnet", "watch", "run", "--project", "Mandys.Api/Mandys.Api.csproj", "--urls", "http://+:8080"]

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first for better layer caching
COPY Mandys.Api/Mandys.Api.csproj Mandys.Api/
COPY Mandys.Application/Mandys.Application.csproj Mandys.Application/
COPY Mandys.Infrastructure/Mandys.Infrastructure.csproj Mandys.Infrastructure/
COPY Mandys.Domain/Mandys.Domain.csproj Mandys.Domain/
COPY Directory.Build.props ./
RUN dotnet restore Mandys.Api/Mandys.Api.csproj

# Copy the rest of the source and publish
COPY . .
RUN dotnet publish Mandys.Api/Mandys.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
USER app
ENTRYPOINT ["dotnet", "Mandys.Api.dll"]
