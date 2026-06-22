# Application Overview

## Summary

`TravelSBE` is an ASP.NET Core Web API backed by PostgreSQL via Entity Framework Core and NetTopologySuite. The application exposes authenticated endpoints for user accounts, objectives, events, experiences, itineraries, reviews, images, and machine-learning-based recommendations.

## Main runtime pieces

- `Program.cs` bootstraps the application, dependency injection container, middleware pipeline, database migrations, default data seeding, and ML initialization.
- `Data/ApplicationDbContext.cs` defines the EF Core model and the relational/geospatial mappings.
- `Controllers/` contains the HTTP API entrypoints.
- `Services/` contains most business logic and data access orchestration.
- `Entity/` contains persistence entities mapped by EF Core.
- `Models/` and `Dtos/` contain request/response models used by controllers and services.
- `Mapper/MappingProfile.cs` configures AutoMapper mappings between entities and API models.
- `wwwroot/uploads/images/` stores uploaded images on disk, while metadata is stored in `ObjectiveImages`.
- `ml_output/` stores generated JSON artifacts used by the recommendation/clustering workflow.

## Startup sequence

The current startup flow in `Program.cs` is:

1. Read configuration from `appsettings.json` and environment sources.
2. Register permissive CORS under the `AllowAll` policy.
3. Configure JWT bearer authentication.
4. Configure controllers with an increased JSON depth limit.
5. Register `ApplicationDbContext` with PostgreSQL and NetTopologySuite enabled.
6. Configure Kestrel to listen on port `7100`.
7. Increase multipart upload size to `100 MB`.
8. Register a global fixed-window rate limiter of `100 requests/minute` per user or IP.
9. Register AutoMapper, `IHttpContextAccessor`, application services, and an `HttpClient` for `ItineraryService`.
10. Build the app and install a global exception handler that returns HTTP `500` with a generic JSON payload.
11. Create a startup scope and execute:
   - `Database.MigrateAsync()`
   - `MLService.TrainModelAsync()`
   - `MLService.UpdateClusterNeighborsAsync()`
   - `ObjectiveService.UpdateMissingLocationsAsync()`
   - `ObjectiveService.InsertDefaultObjectives()`
   - if default objectives were inserted, train and rebuild neighbors again
12. Enable HTTPS redirection, CORS, rate limiting, static file serving, authentication, authorization, and controller routing.

## Middleware behavior

- Authentication uses JWT bearer tokens signed with the configured symmetric key.
- Most controllers are marked with `[Authorize]`.
- `UserController.Login` and `UserController.SignUp` are the main anonymous endpoints.
- Static files are served both from the default `wwwroot` and from `wwwroot/uploads/images`.

## Configuration dependencies

Important configuration currently used by the code:

- `ConnectionStrings:DefaultConnection`
- `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpireMinutes`
- `BaseUrl`
- `MachineLearning:*`
- `Cors:AllowedOrigins` is read, but the active CORS policy currently allows any origin instead.

## Architectural observations

- The code follows a conventional controller -> service -> EF Core flow.
- Services contain both business rules and query composition; there is no separate repository layer.
- Some startup work is synchronous from the perspective of app readiness because migrations, ML training, and neighbor generation run before the app begins accepting requests.
- The project includes a mix of namespaces such as `TravelSBE` and `TravelsBE`, so changes should be made carefully to avoid import confusion.
