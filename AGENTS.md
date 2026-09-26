# Mandys Backend — Agent Instructions

> Backend API for **Mandys**, a fast-food franchise management system.
> ASP.NET Core 10 minimal API · Carter · EF Core · PostgreSQL · JWT auth · Docker.

## Quick Reference

| Item | Value |
|---|---|
| Language | C# 14 / .NET 10 |
| Solution | `Mandys.slnx` (4 projects) |
| Build | `dotnet build Mandys.slnx` |
| Run (dev) | `docker compose -f docker/compose.database.yml up -d database` then `docker compose -f docker/compose.yml up --build api` |
| API base | `http://localhost:8080/api/` |
| API docs (dev) | Scalar UI `http://localhost:8080/scalar/v1`, OpenAPI JSON `/openapi/v1.json` |
| Database | PostgreSQL 18 via Docker, connection in `appsettings.Development.json` |
| Migrations | Auto-applied on dev startup; scaffold with `dotnet ef migrations add <Name> --project Mandys.Infrastructure --startup-project Mandys.Api` |
| CI | GitHub Actions → build Docker image → push to `ghcr.io/umdalecs/mandys-backend` |

## Architecture (Clean Architecture, 4 layers)

```
Mandys.Api  →  Mandys.Application  →  Mandys.Domain
                      ↑
Mandys.Infrastructure ┘
```

| Project | Role | Key contents |
|---|---|---|
| **Mandys.Domain** | Pure domain entities. No EF, no persistence. Guard own invariants. | `User.cs`, `Branch.cs`, `Product.cs`, `Dish.cs`, `Roles.cs` |
| **Mandys.Application** | Use cases, DTOs, service interfaces + implementations, repository *interfaces*, `ServiceException`. | `DTOs/`, `Services/Interfaces/`, `Services/Implementations/` |
| **Mandys.Infrastructure** | EF Core, persistence records, mappers (record↔domain), repository implementations, migrations, security (Argon2). | `ApplicationDbContext.cs`, `Persistence/Records/`, `Persistence/Mappers/`, `Persistence/Repositories/`, `Migrations/` |
| **Mandys.Api** | HTTP layer. Carter handlers (minimal API routes), FluentValidation validators, DI wiring in `Program.cs`. | `Handlers/`, `Validators/`, `Configuration/`, `Program.cs` |

**Dependency rule:** `Application` never references `Infrastructure`. Repositories are consumed via interfaces only.

## Domain Model

### Business Context

Mandys is a **fast-food franchise with multiple branches** (sucursales). Headquarters defines the catalog and rules; branches sell, consume stock, and operate under their own staff.

- **Selling branch**: serves customers (has an address).
- **Warehouse-only branch** (`Branch.WarehouseOnly`): holds inventory, does not sell.

### Roles (`Mandys.Domain/Roles.cs`)

| Role constant | Spanish value | Branch requirement |
|---|---|---|
| `Administrator` | `administrador` | Optional — can use any branch |
| `OpChief` | `gerenteOperaciones` | Required — exactly one branch |
| `BranchChief` | `gerenteSucursal` | Required — exactly one branch |
| `Cashier` | `cajero` | Required — exactly one branch |
| `KitchenChief` | `jefeCocina` | Required — exactly one branch |
| `WarehouseChief` | `encargadoAlmacen` | Required — exactly one branch |
| `CentralWarehouseChief` | `encargadoAlmacenCentral` | Required — exactly one branch |
| `Customer` | `cliente` | Optional — can use any branch |

Branch optionality enforced by `Roles.RequiresBranch()`.

### Products vs Supplies (`Product.IsSupply`)

- `IsSupply = false` → sellable catalog item (menu product), needs `Price`.
- `IsSupply = true` → insumo (ingredient/consumable), priced by purchase cost, measured in stock units.

### Domain Rules (enforce in every new feature)

1. **Branch-scope everything operational.** Stock, sales, orders, shifts always carry a branch id. Branch roles only see/mutate **their own branch**.
2. **Global scope is headquarters only.** Cross-branch reads, catalog management require `administrador` or `gerenteOperaciones`.
3. **Staff belong to one branch** (`UserRecord.BranchId`). Optional only for `administrador` and `cliente`.
4. **Warehouse-only branches don't sell.** Selling flows must check branch kind.
5. **Supplies flow inward, products flow outward.** Central warehouse → branch warehouse → kitchen → sale.

## Coding Conventions

### General

- **Indent**: 4 spaces, LF line endings, UTF-8 (see `.editorconfig`).
- **Nullable reference types**: enabled globally (`Directory.Build.props`).
- **Implicit usings**: enabled.
- Build target: **zero warnings, zero errors** (`dotnet build Mandys.slnx`).

### Domain Entities (`Mandys.Domain/`)

- Plain C# classes, namespace `Mandys.Domain`.
- Properties with `private set`; constructor validates via guards (`throw ArgumentException`).
- Mutation through explicit methods (`UpdateDetails()`, `ChangeEmail()`), never public setters.
- Fields: `Id` (int), business properties, `CreatedAt`, `UpdatedAt`.

### Persistence Records (`Mandys.Infrastructure/Persistence/Records/`)

- Inherit from `Record` base class (gives `CreatedAt`, `UpdatedAt`, `DeletedAt`, `IsDeleted` + soft-delete).
- Namespace `Mandys.Infrastructure.Persistence`.
- Use `[Key]` + `[DatabaseGenerated(Identity)]` for int `Id`.
- Table/column names: **snake_case** is automatic via `UseSnakeCaseNamingConvention()`.
- **Never hard-delete rows** — use soft-delete via `Record` base class.

### Mappers (`Mandys.Infrastructure/Persistence/Mappers/`)

- `internal static class XMapper` with `ToDomain()` and `ToRecord()`.
- Domain ↔ record conversion, keeping Id and audit dates on `ToDomain()`.

### Repositories

- **Interface** in `Mandys.Application/Services/Interfaces/` — domain-in/domain-out.
- **Implementation** in `Mandys.Infrastructure/Persistence/Repositories/`.
- Reads use `AsNoTracking()`.
- `SearchAsync` returns `(TotalCount, Items)` tuple for paging.
- `UpdateAsync` loads tracked record, copies scalar fields (key + audit survive).
- `RemoveAsync` = soft-delete via `FindAsync` + `Remove`.

### DTOs (`Mandys.Application/DTOs/`)

- Namespace `Mandys.DTOs`.
- Pattern: `XResponse`, `CreateXRequest`, `UpdateXRequest` (nullable optionals), `PagedXResponse`.
- Include a `ToResponse()` extension mapper class.

### Services (`Mandys.Application/Services/`)

- Namespace `Mandys.Services`.
- Thin orchestration: clamp paging (`page >= 1`, `1 <= pageSize <= 100`), validate input.
- Throw `ServiceException.BadRequest` / `.NotFound` / `.Conflict` for expected failures.
- Domain mutation via entity methods, then `repository.UpdateAsync`.

### Handlers (`Mandys.Api/Handlers/`)

- `class XHandler : ICarterModule`, namespace `Mandys.Handlers`.
- `MapGroup("/entityname").WithTags("EntityName").RequireAuthorization()`.
- Use `{id:int}` route constraints for int keys.
- Restrict writes: `.RequireAuthorization(policy => policy.RequireRole(Roles.Administrator))`.
- Wrap service calls: `try/catch ServiceException` → map to HTTP status (`400`, `404`, `409`).
- No registration needed — Carter discovers `ICarterModule` via `AddCarter()`.

### Validators (`Mandys.Api/Validators/`)

- FluentValidation validators registered as scoped services in `Program.cs`.
- Bridge to error flow via `RequestValidation.ThrowIfInvalid()` → `ServiceException.BadRequest`.

### DI Wiring (`Mandys.Api/Program.cs`)

When adding a new entity, register:
```csharp
builder.Services.AddScoped<IXRepository, XRepository>();
builder.Services.AddScoped<IXService, XService>();
```
Plus any `IValidator<CreateXRequest>` / `IValidator<UpdateXRequest>`.

### Migrations

- Migrations are **gitignored** and scaffolded locally.
- Scaffold: `dotnet ef migrations add <Name> --project Mandys.Infrastructure --startup-project Mandys.Api`
- In dev, the API auto-applies pending migrations on startup (`Database.Migrate()` in `Program.cs`).
- Full reset: `make migrations-fresh` (revert DB → delete migrations → scaffold Initial → apply).
- **Never edit scaffolded migrations by hand.** Model changes go in records / `OnModelCreating`, then regenerate.

### IDs

- Always `int` identity (`[DatabaseGenerated(Identity)]`).

### Paging Defaults

- `page = 1`, `pageSize = 20`, max `100`.

### Error Handling

- Services throw `ServiceException` (with status code).
- Handlers catch and map to HTTP: `400` → BadRequest, `404` → NotFound, `409` → Conflict.
- Domain guards throw `ArgumentException`; services translate by validating input first.

## Adding a New Entity (Step-by-step)

Follow this exact order (replace `X` with entity name):

1. **Domain entity** → `Mandys.Domain/X.cs`
2. **Persistence record** → `Mandys.Infrastructure/Persistence/Records/XRecord.cs` (extends `Record`)
3. **Mapper** → `Mandys.Infrastructure/Persistence/Mappers/XMapper.cs`
4. **DbSet + migration** → Add `DbSet<XRecord>` to `ApplicationDbContext.cs`, configure relations in `OnModelCreating`, scaffold migration
5. **Repository interface** → `Mandys.Application/Services/Interfaces/IXRepository.cs`
6. **Repository implementation** → `Mandys.Infrastructure/Persistence/Repositories/XRepository.cs`
7. **DTOs** → `Mandys.Application/DTOs/XDtos.cs`
8. **Service interface + implementation** → `Mandys.Application/Services/Interfaces/IXService.cs` + `Services/Implementations/XService.cs`
9. **Handler** → `Mandys.Api/Handlers/XHandler.cs`
10. **DI wiring** → Register in `Program.cs`
11. **Verify** → `dotnet build Mandys.slnx` — 0 warnings, 0 errors

## Docker / Deployment

- **Dockerfile**: `docker/Dockerfile` — multi-stage (SDK build → Alpine runtime).
- **Dev compose**: `docker/compose.yml` (api) + `docker/compose.database.yml` (PostgreSQL 18).
- **Env vars**: see `docker/.env.example`. Key: `ConnectionStrings__DefaultConnection`, `Jwt__Key`, `Frontend__AllowedOrigins`.
- **CI**: GitHub Actions on push to `main` / tags → build + push to GHCR.
- **Cross-origin frontend** (Cloudflare Pages): set `Frontend__AllowedOrigins`, `Auth__CookieSameSite=None`, `Auth__CookieSecure=Always`.

## Key Packages

| Package | Purpose |
|---|---|
| Carter | Minimal API module discovery (`ICarterModule`) |
| FluentValidation | Request DTO validation |
| EF Core + Npgsql | ORM + PostgreSQL provider |
| EFCore.NamingConventions | Automatic snake_case table/column names |
| Isopoh.Cryptography.Argon2 | Password hashing |
| System.IdentityModel.Tokens.Jwt | JWT token generation/validation |
| Scalar.AspNetCore | OpenAPI docs UI (dev only) |
