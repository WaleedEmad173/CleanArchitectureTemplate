# Clean Architecture .NET 10 API Template

A reusable ASP.NET Core API starter with a clear split between the application use cases and infrastructure providers.

## Included

- .NET 10 Web API, Clean Architecture, CQRS, and MediatR
- FluentValidation pipeline and consistent API responses
- ASP.NET Core Identity, JWT access tokens, and rotating refresh tokens
- EF Core with SQL Server migrations and soft deletion
- Product feature demonstrating filtering, search, sorting, pagination, and response caching
- Global exception handling, CORS, rate limits, forwarded headers, and Swagger

## Dependency licensing

MediatR is distributed under the Reciprocal Public License 1.5, with an alternative commercial license available. Review the [MediatR license terms](https://github.com/LuckyPennySoftware/MediatR/blob/main/LICENSE.md) before reusing this template in a closed-source product.

If you use a MediatR license key, supply it as the `MEDIATR_LICENSE_KEY` environment variable or through your deployment secret store; do not commit it.

## Start locally

1. Install the .NET 10 SDK and have a SQL Server instance available.
2. Copy `src/CleanArchitectureTemplate.API/appsettings.example.json` to `src/CleanArchitectureTemplate.API/appsettings.json`. Update the connection string for your SQL Server instance. The example uses a new database name so its first migration does not collide with tables in an older database.
3. Configure a local JWT signing key with User Secrets if you want tokens to remain valid after restarting the API:

   ```powershell
   dotnet user-secrets set "Jwt:Key" "<private random secret of at least 32 bytes>" --project src/CleanArchitectureTemplate.API/CleanArchitectureTemplate.API.csproj
   ```

   Development can start without this value; it generates an ephemeral key. Tokens signed with that key stop working after restart. Production must always provide a private signing key.
4. Configure SMTP settings before using registration OTP or password reset. Configure `GoogleAuth:ClientId` before using Google login.
5. Start the API:

   ```powershell
   dotnet run --project src/CleanArchitectureTemplate.API/CleanArchitectureTemplate.API.csproj
   ```

   Development applies EF Core migrations and seeds the `User` and `Admin` roles during startup. Swagger is available at the HTTPS address in `Properties/launchSettings.json`.

Use a new, empty database for the first migration. If the target database already has application tables but no matching EF migration history, point the connection string at a new database or reconcile the schema and migration history before starting. Do not delete an existing database just to clear a migration conflict.

## Production deployment

- Production does not apply migrations at API startup by default. Apply them in the deployment step before starting the API:

  ```powershell
  dotnet ef database update --project src/CleanArchitectureTemplate.Infrastructure/CleanArchitectureTemplate.Infrastructure.csproj --startup-project src/CleanArchitectureTemplate.API/CleanArchitectureTemplate.API.csproj
  ```

- Set `Database:ApplyMigrationsOnStartup=true` only when you deliberately want the API process to run migrations. For multi-instance deployments, use a single migration step instead of having every API instance migrate concurrently.
- Supply the connection string, JWT settings, SMTP credentials, Google client ID, and allowed frontend origins through the deployment environment or secret store. Do not deploy with Development settings enabled.
- Set `Cors:AllowedOrigins` to exact frontend origins. Configure `ForwardedHeaders:KnownProxies` with only trusted reverse proxy IP addresses.
- Product creation, updates, and deletion require the `Admin` role. Startup creates the `Admin` role but does not assign it to any account; provision the initial administrator through a controlled deployment process before granting access to those endpoints.

## Extending the template

Product is the sample business feature. To add a feature:

1. Add the domain entity and its EF configuration under `Infrastructure/Context/Configurations`.
2. Add a migration and apply it to the target database.
3. Add command/query contracts, validators, handlers, and DTOs under `Application/Features`.
4. Keep EF Core queries in Infrastructure behind an application interface, following `IProductQueryService` and `ProductQueryService`.
5. Add a repository interface and implementation only when a use case needs entity-specific persistence operations. Update `IUnitOfWork` for repositories used by write workflows.
6. Add an API controller endpoint and a request example in the `.http` file.

Registration OTPs, response caching, cache invalidation, and rate limiting currently use process-local memory. They work on a single API instance. For multiple instances, use shared distributed storage for OTPs and cache invalidation, and a distributed rate limiter. Configure a persistent, shared ASP.NET Core Data Protection key ring as well so password reset tokens continue to validate across restarts and instances. Memory cache entries are size-limited; set a size on any new entries added to the shared cache.
