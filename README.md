# Clean Architecture .NET 10 Template

A reusable backend template designed for new projects.

## Stack
- .NET 10 Web API
- Clean Architecture
- CQRS + MediatR
- FluentValidation Pipeline
- Generic Repository
- Unit of Work
- ASP.NET Core Identity
- JWT Access Token
- Refresh Token rotation
- Global Exception Middleware
- Unified API Response
- Pagination
- Swagger

## The idea
Authentication and infrastructure are already ready.
Product is the **only business controller/feature guide**.

For a new project, normally you only need to:
1. Add your Entity.
2. Add Entity Configuration in `Infrastructure/Context/AppDbContext.cs` (or move it to a configuration class).
3. Add your repository interface if you need entity-specific queries.
4. Add the repository implementation if needed.
5. Add the repository property to `IUnitOfWork` and `UnitOfWork`.
6. Copy `Application/Features/Products` and rename it to your feature.
7. Copy `ProductController` and rename it.

Do not rebuild authentication, JWT, refresh tokens, middleware, validation pipeline, generic repository, or UnitOfWork for every project.

## Run
1. Open the solution in Visual Studio 2022 with the .NET 10 SDK installed.
2. Set `CleanArchitectureTemplate.API` as Startup Project.
3. Check `appsettings.json` connection string and JWT key.
4. Run.
5. Swagger opens with the API endpoints.

The template uses `EnsureCreated()` so the database can be created automatically for a first run. For a real production project, replace this with EF Core migrations.
