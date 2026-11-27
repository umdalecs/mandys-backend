# MandysBackend

Backend API for Mandys — ASP.NET Core 10 minimal API with JWT auth, PostgreSQL, and S3-compatible object storage (RustFS).

## Project Structure

The solution (`Mandys.slnx`) follows Clean Architecture across four layers plus a seeding client:

- **`Mandys.Domain`** — Pure domain entities, value guards, measure units, and role definitions.
- **`Mandys.Application`** — Use cases, DTOs, service interfaces and implementations, repository interfaces.
- **`Mandys.Infrastructure`** — EF Core (`ApplicationDbContext`), persistence records, mappers, repositories, Argon2 password hashing, S3 storage service.
- **`Mandys.Api`** — Carter minimal API handlers, FluentValidation validators, DI & auth configuration.
- **`Mandys.Seeding`** — Idempotent .NET console seeder that populates catalog, branch, user, and inventory data over HTTP.

## Development

### Prerequisites

- Docker + Docker Compose plugin
- .NET 10 SDK (optional if running everything via Docker; required for local `dotnet run`, EF migrations, and `Mandys.Seeding`)

### Setup

```bash
cp .env.example .env
```

`.env` is optional (every variable has a working default) and gitignored. It holds database credentials (`POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB`, `DB_PORT`), S3 storage settings (`S3_*`), pgAdmin settings (`PGADMIN_*`), and API settings (`API_PORT`, `DB_HOST`).

### Run the application

#### Option A: Full stack in Docker

Start PostgreSQL, RustFS (S3 storage + bucket init), pgAdmin, and the API container:

```bash
docker compose up -d
```

The API applies pending migrations itself on startup in `Development` (creates tables + seeds default users), so no SDK or `dotnet-ef` is needed. There is no hot reload in the container: after pulling backend changes, re-run with `--build`.

#### Option B: Local API + Docker infrastructure (`Makefile`)

If you have the .NET 10 SDK installed, you can run PostgreSQL and RustFS in Docker and launch the API locally:

```bash
make run          # starts database, storage, and runs Mandys.Api
make stop         # stops and removes database and storage containers/volumes
```

Other useful `Makefile` targets:

| Target | Description |
| ------ | ----------- |
| `make run-database` / `make stop-database` | Start or stop (with `-v`) the PostgreSQL container |
| `make run-storage` / `make stop-storage` | Start or stop (with `-v`) RustFS and bucket initialization |
| `make run-pgadmin` / `make stop-pgadmin` | Start or stop pgAdmin 4 |
| `make run-api` | Run `Mandys.Api` locally via `dotnet run` |
| `make seed` | Run `Mandys.Seeding` against `BASE_URL` (default `http://localhost:8080`) |
| `make fresh-migrations` | Revert DB to 0, delete migration files, scaffold `Initial`, and apply |

The API is available at `http://localhost:8080` (`API_PORT` in `.env` changes the host port).
API docs (`Development` only):
- Scalar UI: `http://localhost:8080/scalar/v1`
- OpenAPI JSON: `http://localhost:8080/openapi/v1.json`

### Delete everything

```bash
docker compose down -v
```

(Add `-v` only when you want to wipe the database and storage volumes.)

### Object Storage (RustFS S3)

Product, dish, and combo images are stored in an S3-compatible bucket (`mandys` by default). In development, `compose.yml` runs **RustFS** and automatically initializes a public-read bucket via `storage-init`:

- S3 API endpoint: `http://localhost:9000`
- RustFS Console UI: `http://localhost:9001` (default credentials: `rustfsadmin` / `rustfsadmin`)

### pgAdmin 4 (browse the database / ERD)

Open `http://localhost:5050` and log in with `admin@mandyspos.com` / `administrator` (or the values of `PGADMIN_EMAIL` / `PGADMIN_PASSWORD` / `PGADMIN_PORT` in `.env`). Register the server once: host `database`, port `5432`, maintenance database `mandys`, username `postgres`, password `postgres`. Use **Tools → ERD Tool** for the diagram, and **Tools → Query Tool** to inspect data.

Stop it with `make stop-pgadmin` or `docker compose down pgadmin -v`.

### Verify it works

Default seeded accounts use password `mandyspos` (`admin@mandyspos.com`, `operaciones@mandyspos.com`, `cliente@mandyspos.com`):

```bash
curl -i -c cookies.txt -X POST http://localhost:8080/api/login \
  -H 'Content-Type: application/json' \
  -d '{"email":"admin@mandyspos.com","password":"mandyspos"}'

curl -b cookies.txt http://localhost:8080/api/users/me
```

## Seeding catalog data

Migrations seed the base `admin@mandyspos.com`, `operaciones@mandyspos.com`, and demo `cliente@mandyspos.com` users (password: `mandyspos`). Branches, staff users, products, dishes, combos, and warehouse storage movements are seeded over HTTP using the `Mandys.Seeding` project (idempotent):

```bash
make seed
# or directly:
dotnet run --project Mandys.Seeding -- http://localhost:8080
```

## Run without cloning (from GHCR)

You only need Docker. Copy the exact image path from the package page (`ghcr.io/umdalecs/mandys-backend:<tag>`); the examples below use `ghcr.io/umdalecs/mandys-backend:latest`.

The API listens on port `8080` and accepts the following configuration variables:

| Variable | Purpose | Example |
| -------- | ------- | ------- |
| `ConnectionStrings__DefaultConnection` | Postgres connection string | `User ID=postgres;Password=postgres;Host=<db-host>;Port=5432;Database=mandys;` |
| `Jwt__Key` | Token signing key, 32+ bytes (**required**, no usable default) | `change-me-to-a-32-plus-byte-secret` |
| `S3__Endpoint` | S3 service URL for server-side uploads | `http://storage:9000` |
| `S3__PublicUrl` | Public base URL used to build image links returned by the API | `http://localhost:9000` |
| `S3__AccessKey` / `S3__SecretKey` | S3 credentials | `rustfsadmin` |
| `S3__BucketName` | Target S3 bucket name | `mandys` |
| `S3__Region` | S3 authentication region | `us-east-1` |
| `S3__ForcePathStyle` | Path-style addressing (`true` for RustFS/MinIO) | `true` |
| `Frontend__AllowedOrigins` | Frontend origin(s), comma-separated (CORS + CSRF origin check; localhost defaults apply when empty) | `https://mandys.pages.dev` |
| `Auth__CookieSameSite` | Cookie policy: `Lax` (same-site default), `None` (cross-site), `Strict` | `None` |
| `Auth__CookieSecure` | `Auto` (Secure on HTTPS, default) or `Always` | `Always` |

> The database must already be migrated (tables + seeded users).
> Migrating requires the repo once — see [Run the application](#run-the-application). All compose commands below run from the repo root

### With compose

Save this as `compose.yml` anywhere and run `docker compose up -d`:

```yaml
services:
  database:
    image: postgres:18.6
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
  -d '{"email":"admin@mandyspos.com","password":"mandyspos"}'
```

(The Scalar/OpenAPI docs UI is only enabled in `Development`, so it is not available when running with `ASPNETCORE_ENVIRONMENT=Production`.)

### Frontend on a separate server (Cloudflare Pages/Workers)

The frontend is a static SPA, so it can live anywhere — point its `VITE_API_URL` build variable at the public API URL (e.g. `https://api.mandys.com/api`). On the API side, three things change versus same-site development:

1. **CORS/CSRF origins**: set `Frontend__AllowedOrigins` to the public frontend origin(s), e.g. `Frontend__AllowedOrigins=https://mandys.pages.dev`. Both the CORS policy and the refresh/logout CSRF origin check enforce it. (Any `localhost` origin stays allowed for development.)
2. **Cookies**: browsers only send cookies cross-site with `SameSite=None` + `Secure`, so set `Auth__CookieSameSite=None` and `Auth__CookieSecure=Always`. This requires HTTPS in front of the API (the app honors `X-Forwarded-Proto` from the proxy for `IsHttps`).
3. **CSRF**: the `XSRF-TOKEN` double-submit cookie is not readable by JS across sites, so refresh/logout additionally accept a browser-controlled `Origin`/`Referer` matching the allow-list. No frontend change needed (`withCredentials` is already set).

```bash
docker run -d --name mandys-api --network mandys -p 8080:8080 \
  -e "ConnectionStrings__DefaultConnection=User ID=postgres;Password=postgres;Host=mandys-db;Port=5432;Database=mandys;" \
  -e "Jwt__Key=change-me-to-a-32-plus-byte-secret" \
  -e "Frontend__AllowedOrigins=https://mandys.pages.dev" \
  -e "Auth__CookieSameSite=None" \
  -e "Auth__CookieSecure=Always" \
  ghcr.io/umdalecs/mandys-backend:latest
```
