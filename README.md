# MandysBackend

Backend API for Mandys — run it with Docker + Compose (no local .NET SDK needed).

## Prerequisites

- Docker + Docker Compose plugin

## Setup

```bash
cp .env.example .env
```

`.env` is optional (every variable has a working default) and gitignored. It
holds database credentials (`POSTGRES_USER`, `POSTGRES_PASSWORD`,
`POSTGRES_DB`, `DB_PORT`) and api settings (`API_PORT`,
`ASPNETCORE_ENVIRONMENT`, `DB_HOST`).

## Run with the database

1. Start the shared development database (data persists in the
   `database-data` volume):

   ```bash
   docker compose -f compose.database.yml up -d database
   ```

2. Apply migrations (creates tables + seeds the admin user):

   ```bash
   docker compose exec api dotnet-ef database update --project Mandys/Mandys.csproj
   ```

   Or from the host if you have the .NET SDK + `dotnet-ef` installed:

   ```bash
   dotnet ef database update --project Mandys
   ```

3. Run the api (development mode, hot reload):

   ```bash
   docker compose up --build api
   ```

The api is at `http://localhost:8080` (`API_PORT` in `.env` changes the host
port). API docs (Development only): Scalar UI
`http://localhost:8080/scalar/v1`, OpenAPI JSON
`http://localhost:8080/openapi/v1.json`.

## Run modes (one shared database)

| Mode | Command | When to use |
| ---- | ------- | ----------- |
| Api only (default) | `docker compose -f compose.database.yml up -d database` once, then `docker compose up --build api` | Daily development: restart/rebuild the api without touching the database |
| Api + database | `docker compose -f compose.yml -f compose.database.yml up --build` | Fresh checkout / CI: everything from zero, api waits for a healthy database |

Both modes share the `database-data` volume, so the data is the same either
way. Stop everything with:

```bash
docker compose -f compose.yml -f compose.database.yml down
```

(add `-v` only to wipe the database).

## Verify it works

```bash
curl -i -X POST http://localhost:8080/api/login \
  -H 'Content-Type: application/json' \
  -d '{"email":"admin@mandys.com","password":"<seeded-password>"}'

curl -b cookies.txt -c cookies.txt http://localhost:8080/api/users/me
```
