# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Common commands

- Restore dependencies and build the whole solution (run from repo root):
  - `dotnet restore`
  - `dotnet build Speculo.sln`
- Run the Web API locally (defaults to Kestrel on the configured port):
  - `dotnet run --project Speculo.API`
- Apply the latest EF Core migrations to the configured PostgreSQL database (DbContext in `Speculo.Infrastructure`, startup in `Speculo.API`):
  - `dotnet ef database update --project Speculo.Infrastructure --startup-project Speculo.API`
- Add a new EF Core migration after changing the domain entities or configurations:
  - `dotnet ef migrations add <MigrationName> --project Speculo.Infrastructure --startup-project Speculo.API`
- Run tests:
  - There are currently no test projects checked in. Once test projects exist (for example `Speculo.Tests`), run all tests from the repo root with `dotnet test`.
  - To run a single test (once tests exist), use a filter:
    - `dotnet test --filter "FullyQualifiedName=Namespace.ClassName.MethodName"`

### Configuration and secrets

- The API project (`Speculo.API`) targets .NET 9 (`net9.0`) and uses ASP.NET Core Web API.
- Database access is configured in `Speculo.Infrastructure.DependencyInjection.AddInfrastructure` via:
  - `services.AddDbContext<SpeculoDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")))`
- JWT settings are bound in `Program.cs` from the `JwtSettings` configuration section and mapped to `Speculo.Infrastructure.Authentication.JwtSettings`.
- `Speculo.API.csproj` defines a `UserSecretsId`, so during local development you can use ASP.NET Core User Secrets to store `JwtSettings` and the `DefaultConnection` connection string.

## High-level architecture

This is a layered clean-architecture-style .NET solution centered around an event-sourced core for tracking user activity and mood.

### Projects and responsibilities

- `Speculo.Domain` (core model)
  - Contains persistence-agnostic domain entities and events.
  - Key entities:
    - `User`: authentication identity, email, password hash, full name, registration timestamp.
    - `Event`: generic persisted event with `Id`, `UserId`, `Type`, JSON `Payload`, and `Timestamp`.
    - `DailyAggregate`: per-user, per-day summary (money spent, work hours, sleep, mood metrics) keyed by (`UserId`, `Date`).
  - Domain events:
    - `IDomainEvent` defines an `OccurredOn` timestamp.
    - `MoodLoggedEvent` represents a user logging their mood (user id, score, notes) and implements `IDomainEvent`.

- `Speculo.Application` (use cases and orchestration)
  - Depends only on `Speculo.Domain` and defines the application boundary via interfaces and MediatR handlers.
  - Registers itself via `AddApplication(this IServiceCollection services)` which configures MediatR for the assembly.
  - Interfaces describing what the infrastructure must provide:
    - `ISpeculoDbContext`: abstraction over the EF Core context exposing `DbSet<User>`, `DbSet<Event>`, `DbSet<DailyAggregate>` plus `SaveChangesAsync`.
    - `IIdentityService`: registration and login flows returning `AuthResponse`.
    - `IJwtTokenGenerator`: creates JWTs from a `User`.
    - `ICurrentUserProvider`: gives the current user id and authentication status based on the current request.
    - `IEventStore`: persists and retrieves domain events.
  - Example feature slice (`Features/Events/Commands/LogMood`):
    - `LogMoodCommand` is a MediatR `IRequest<Guid>` containing the mood score and optional notes.
    - `LogMoodCommandHandler` composes the domain and infrastructure via interfaces:
      - Reads the current user id from `ICurrentUserProvider`.
      - Creates a `MoodLoggedEvent` domain event.
      - Persists it via `IEventStore.SaveAsync`, returning the created event id.

- `Speculo.Infrastructure` (persistence, identity, and external concerns)
  - Implements the interfaces from `Speculo.Application` and owns all concrete framework dependencies.
  - Dependency injection wiring via `AddInfrastructure(this IServiceCollection services, IConfiguration configuration)`:
    - Registers `SpeculoDbContext` using Npgsql and the `DefaultConnection` connection string.
    - Exposes `SpeculoDbContext` as `ISpeculoDbContext`.
    - Registers `IdentityService` as `IIdentityService`.
    - Configures `JwtSettings` from configuration and registers `JwtTokenGenerator` as `IJwtTokenGenerator`.
    - Registers `EventStore` as `IEventStore`.
    - Adds `IHttpContextAccessor` and binds `ICurrentUserProvider` to `CurrentUserProvider`.
  - `SpeculoDbContext` implements `ISpeculoDbContext`, defines the `DbSet<>`s, and applies all `IEntityTypeConfiguration<>` mappings from the assembly.
  - Entity configurations:
    - `UserConfiguration`: enforces keys, uniqueness on email, required properties, and max lengths.
    - `EventConfiguration`: configures `Event` with an index on `UserId` and stores `Payload` as PostgreSQL `jsonb`.
    - `DailyAggregateConfiguration`: composite key (`UserId`, `Date`) and precision for `TotalSpent`.
  - Services:
    - `IdentityService` handles registration and login using EF Core (`ISpeculoDbContext`) and BCrypt for password hashing, then issues JWTs via `IJwtTokenGenerator`.
    - `JwtTokenGenerator` builds JWTs using `JwtSettings` and includes standard claims (`sub`, `email`, `name`, `jti`).
    - `CurrentUserProvider` reads the current user from the HTTP context (`ClaimTypes.NameIdentifier` or `sub`) and exposes `UserId`/`IsAuthenticated` to the application layer.
    - `EventStore` implements the event-sourcing persistence pattern by serializing `IDomainEvent` instances to JSON and storing them in the `Events` table; retrieval is currently a placeholder that should be expanded when event replay is implemented.
  - Migrations in `Speculo.Infrastructure.Migrations` define the PostgreSQL schema for `Users`, `Events`, and `DailyAggregates`.

- `Speculo.API` (HTTP boundary)
  - ASP.NET Core Web API project that wires up controllers, authentication, and the application/infrastructure layers.
  - `Program.cs`:
    - Calls `AddControllers`, `AddApplication`, and `AddInfrastructure` on the service collection.
    - Binds `JwtSettings` from configuration and configures JWT Bearer authentication using `JwtBearerDefaults.AuthenticationScheme`.
    - Enables authorization and OpenAPI (`AddOpenApi`).
    - Configures the middleware pipeline: authentication, authorization, then `MapControllers()`.
  - Controllers:
    - `AuthController` (`/api/auth`): exposes `POST /register` and `POST /login`, delegating to `IIdentityService` and returning `AuthResponse` (including JWT).
    - `AccountController` (`/api/account`): an `[Authorize]` endpoint `GET /me` that echoes back user data from JWT claims to validate token wiring.
    - `EventController` (`/api/event`): an `[Authorize]` controller with `POST /mood` that accepts `LogMoodCommand` and sends it through MediatR (`ISender`).

### Typical request flow (example: logging a mood)

1. Client calls `POST /api/event/mood` with a valid JWT and the mood payload.
2. `EventController.LogMood` receives the request and sends a `LogMoodCommand` via MediatR.
3. `LogMoodCommandHandler` constructs a `MoodLoggedEvent` using the current user id from `ICurrentUserProvider` and the request data.
4. `EventStore.SaveAsync` serializes the domain event to JSON and writes an `Event` row through `ISpeculoDbContext`/`SpeculoDbContext`.
5. EF Core persists the change to PostgreSQL using the configured model and migrations.

When extending the system, follow this pattern: keep domain rules in `Speculo.Domain`, application orchestration and MediatR handlers in `Speculo.Application`, framework-specific implementations (EF Core, JWT, HTTP context) in `Speculo.Infrastructure`, and thin HTTP endpoints in `Speculo.API` that delegate into the application layer.