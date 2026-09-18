# MandysBackend

Backend API for Mandys — ASP.NET Core 10 minimal API with JWT auth and PostgreSQL.

## Run without cloning (from GHCR)

You only need Docker. Copy the exact image path from the package page
(`ghcr.io/<owner>/<repo>:<tag>`); the examples below use
`ghcr.io/<owner>/<repo>:latest`.

The api listens on port `8080` and needs two settings:

| Variable | Purpose | Example |
| -------- | ------- | ------- |
| `ConnectionStrings__DefaultConnection` | Postgres connection string | `User ID=postgres;Password=postgres;Host=<db-host>;Port=5432;Database=mandys;` |
| `Jwt__Key` | Token signing key, 32+ bytes (**required**, no usable default) | `change-me-to-a-32-plus-byte-secret` |

> The database must already be migrated (tables + seeded admin user).
> Migrating requires the repo once — see [Run with the database](#run-with-the-database).

### With the docker CLI

```bash
docker network create mandys

docker run -d --name mandys-db --network mandys \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=mandys \
  -v mandys-data:/var/lib/postgresql \
  postgres:18

docker run -d --name mandys-api --network mandys -p 8080:8080 \
  -e "ConnectionStrings__DefaultConnection=User ID=postgres;Password=postgres;Host=mandys-db;Port=5432;Database=mandys;" \
  -e "Jwt__Key=change-me-to-a-32-plus-byte-secret" \
  ghcr.io/<owner>/<repo>:latest
```

### With compose

Save this as `compose.yml` anywhere and run `docker compose up -d`:

```yaml
services:
  database:
    image: postgres:18
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
      POSTGRES_DB: mandys
    volumes:
      - mandys-data:/var/lib/postgresql
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres -d mandys"]
      interval: 5s
      timeout: 5s
      retries: 10

  api:
    image: ghcr.io/<owner>/<repo>:latest
    ports:
      - 8080:8080
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ConnectionStrings__DefaultConnection: "User ID=postgres;Password=postgres;Host=database;Port=5432;Database=mandys;"
      Jwt__Key: change-me-to-a-32-plus-byte-secret
    depends_on:
      database:
        condition: service_healthy

volumes:
  mandys-data:
```

### Verify it works

```bash
curl -i -X POST http://localhost:8080/api/login \
  -H 'Content-Type: application/json' \
  -d '{"email":"admin@mandys.com","password":"<seeded-password>"}'
```

(The Scalar/OpenAPI docs UI is only enabled in Development, so it is not
available on this image.)

## Development

### Prerequisites

- Docker + Docker Compose plugin

### Setup

```bash
cp .env.example .env
```

`.env` is optional (every variable has a working default) and gitignored. It
holds database credentials (`POSTGRES_USER`, `POSTGRES_PASSWORD`,
`POSTGRES_DB`, `DB_PORT`) and api settings (`API_PORT`,
`ASPNETCORE_ENVIRONMENT`, `DB_HOST`).

### Run with the database

1. Start the shared development database (data persists in the
   `database-data` volume):

   ```bash
   docker compose -f compose.database.yml up -d database
   ```

2. Apply migrations (creates tables + seeds the admin user):

   ```bash
   docker compose exec api dotnet-ef database update --project Mandys.Infrastructure/Mandys.Infrastructure.csproj
   ```

   Or from the host if you have the .NET SDK + `dotnet-ef` installed:

   ```bash
   dotnet ef database update --project Mandys.Infrastructure --startup-project Mandys.Api
   ```

3. Run the api (development mode, hot reload):

   ```bash
   docker compose up --build api
   ```

The api is at `http://localhost:8080` (`API_PORT` in `.env` changes the host
port). API docs (Development only): Scalar UI
`http://localhost:8080/scalar/v1`, OpenAPI JSON
`http://localhost:8080/openapi/v1.json`.

### Run modes (one shared database)

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

### Verify it works

```bash
curl -i -X POST http://localhost:8080/api/login \
  -H 'Content-Type: application/json' \
  -d '{"email":"admin@mandys.com","password":"<seeded-password>"}'

curl -b cookies.txt -c cookies.txt http://localhost:8080/api/users/me
```
