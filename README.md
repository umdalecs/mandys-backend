# MandysBackend

Backend API for Mandys — ASP.NET Core 10 minimal API with JWT auth and PostgreSQL.

## Development

### Prerequisites

- Docker + Docker Compose plugin

### Setup

```bash
cp docker/.env.example docker/.env
```

`docker/.env` is optional (every variable has a working default) and gitignored. It
holds database credentials (`POSTGRES_USER`, `POSTGRES_PASSWORD`,
`POSTGRES_DB`, `DB_PORT`) and api settings (`API_PORT`, `DB_HOST`).

### Run the application

1. Start the shared development database (data persists in the
   `database-data` volume):

   ```bash
   docker compose -f docker/compose.yml up -d
   ```
   The api applies pending migrations itself on startup in Development
   (creates tables + seeds the admin user), so no SDK or `dotnet-ef` is
   needed. Backend developers with the .NET SDK can still run migrations
   from the host instead:

   ```bash
   dotnet ef database update --project Mandys.Infrastructure --startup-project Mandys.Api
   ```

   There is no hot reload: after pulling backend changes, re-run with
   `--build`.

The api is at `http://localhost:8080` (`API_PORT` in `.env` changes the host
port). API docs (Development only): Scalar UI
`http://localhost:8080/scalar/v1`, OpenAPI JSON
`http://localhost:8080/openapi/v1.json`.

### Delete everything

```bash
docker compose -f docker/compose.yml -f down -v
```

(add `-v` only to wipe the database).

### Optional: pgAdmin 4 (browse the database / ERD)

Behind the `tools` profile, so the run modes above are unaffected:

```bash
docker compose -f docker/compose.yml --profile tools up -d pgadmin
```

Open `http://localhost:5050` and log in with `admin@mandys.com` / `postgres`
(override with `PGADMIN_EMAIL` / `PGADMIN_PASSWORD` / `PGADMIN_PORT` in
`docker/.env`). Register the server once: host `database`, port `5432`,
maintenance database `mandys`. Use **Tools → ERD Server** for the diagram, and
**Tools → Query Tool** to inspect data.

Stop it with `docker compose -f docker/compose.yml --profile tools stop pgadmin`.

### Verify it works

```bash
curl -i -X POST http://localhost:8080/api/login \
  -H 'Content-Type: application/json' \
  -d '{"email":"admin@mandyspos.com","password":"administrator"}'

curl -b cookies.txt -c cookies.txt http://localhost:8080/api/users/me
```

## Run without cloning (from GHCR)

You only need Docker. Copy the exact image path from the package page
(`ghcr.io/umdalecs/mandys-backend:<tag>`); the examples below use
`ghcr.io/umdalecs/mandys-backend:latest`.

The api listens on port `8080` and needs two settings:

| Variable | Purpose | Example |
| -------- | ------- | ------- |
| `ConnectionStrings__DefaultConnection` | Postgres connection string | `User ID=postgres;Password=postgres;Host=<db-host>;Port=5432;Database=mandys;` |
| `Jwt__Key` | Token signing key, 32+ bytes (**required**, no usable default) | `change-me-to-a-32-plus-byte-secret` |
| `Frontend__AllowedOrigins` | Frontend origin(s), comma-separated (CORS + CSRF origin check; localhost defaults apply when empty) | `https://mandys.pages.dev` |
| `Auth__CookieSameSite` | Cookie policy: `Lax` (same-site default), `None` (cross-site), `Strict` | `None` |
| `Auth__CookieSecure` | `Auto` (Secure on HTTPS, default) or `Always` | `Always` |

> The database must already be migrated (tables + seeded admin user).
> Migrating requires the repo once — see [Run with the database](#run-the-application). All compose
> commands below run from the repo root; the compose files live in `docker/`.

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
    image: ghcr.io/umdalecs/mandys-backend:latest
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
  -d '{"email":"admin@mandyspos.com","password":"administrator"}'
```

(The Scalar/OpenAPI docs UI is only enabled in Development, so it is not
available on this image.)

### Frontend on a separate server (Cloudflare Pages/Workers)

The frontend is a static SPA, so it can live anywhere — point its
`VITE_API_URL` build variable at the public api URL (e.g.
`https://api.mandys.com/api`). On the api side, three things change versus
same-site development:

1. **CORS/CSRF origins**: set `Frontend__AllowedOrigins` to the public
   frontend origin(s), e.g. `Frontend__AllowedOrigins=https://mandys.pages.dev`.
   Both the CORS policy and the refresh/logout CSRF origin check enforce it.
   (Any `localhost` origin stays allowed for development.)
2. **Cookies**: browsers only send cookies cross-site with
   `SameSite=None` + `Secure`, so set `Auth__CookieSameSite=None` and
   `Auth__CookieSecure=Always`. This requires HTTPS in front of the api
   (the app honors `X-Forwarded-Proto` from the proxy for `IsHttps`).
3. **CSRF**: the `XSRF-TOKEN` double-submit cookie is not readable by JS
   across sites, so refresh/logout additionally accept a browser-controlled
   `Origin`/`Referer` matching the allow-list. No frontend change needed
   (`withCredentials` is already set).

```bash
docker run -d --name mandys-api --network mandys -p 8080:8080 \
  -e "ConnectionStrings__DefaultConnection=User ID=postgres;Password=postgres;Host=mandys-db;Port=5432;Database=mandys;" \
  -e "Jwt__Key=change-me-to-a-32-plus-byte-secret" \
  -e "Frontend__AllowedOrigins=https://mandys.pages.dev" \
  -e "Auth__CookieSameSite=None" \
  -e "Auth__CookieSecure=Always" \
  ghcr.io/umdalecs/mandys-backend:latest
```
