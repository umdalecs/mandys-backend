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
ENTRYPOINT ["dotnet", "watch", "run", "--project", "Mandys/Mandys.csproj", "--urls", "http://+:8080"]

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first for better layer caching
COPY Mandys/Mandys.csproj Mandys/
RUN dotnet restore Mandys/Mandys.csproj

# Copy the rest of the source and publish
COPY . .
RUN dotnet publish Mandys/Mandys.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
USER app
ENTRYPOINT ["dotnet", "Mandys.dll"]
