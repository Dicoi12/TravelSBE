# Request Flow

## Standard flow

Most requests follow the same path:

1. An HTTP request enters a controller in `Controllers/` or `TravelSBE/Controllers/`.
2. Authentication and authorization run before the action if the endpoint is protected.
3. The controller delegates directly to a service interface or service class.
4. The service queries or updates `ApplicationDbContext`.
5. Entities are either mapped with AutoMapper or projected manually into response models.
6. The controller returns the service result or an action result wrapper.

## Layer responsibilities

### Controllers

- Define routes and HTTP verbs.
- Apply `[Authorize]` and `[AllowAnonymous]`.
- Perform only light orchestration.
- Usually return `ServiceResult<T>` or direct DTOs.

### Services

- Hold business logic, validation, and EF Core query composition.
- Construct image URLs using `BaseUrl`.
- Use `CurrentUserService` when user context is needed.
- In some cases trigger cross-cutting actions such as ML retraining after objective changes.

### Persistence

- `ApplicationDbContext` is the single EF Core context.
- Relationships are configured centrally in `OnModelCreating`.
- Geospatial coordinates use `Point` columns stored as `geography(point)`.

## Authentication flow

- `UserService.Login` validates credentials using `PasswordHelper`.
- On success it generates a JWT containing:
  - `NameIdentifier` = user id
  - `Email`
  - `Name` = username
- `CurrentUserService` reads the authenticated principal through `IHttpContextAccessor`.
- Services such as `ItineraryService` and `UserService.ChangePassword` use `CurrentUserService` to resolve the active user.

## Notable service patterns

### Objective flow

- `ObjectivesController` delegates to `ObjectiveService`.
- `ObjectiveService` loads objectives with related images, reviews, and objective types.
- Local objective search uses geospatial filtering and distance ordering.
- Creating an objective can auto-generate the description via a local LLM endpoint if the description is missing.
- After objective creation the service retrains the ML model and rebuilds neighbor data.

### Itinerary flow

- `ItineraryService` creates the itinerary header first, saves it, then inserts `ItineraryDetail` rows.
- Current-user ownership checks are enforced in update and delete operations.
- Read endpoints eagerly load nested objective/event data and convert related images to URLs.

### Image flow

- `ObjectiveImageService` saves uploaded files to disk under `wwwroot/uploads/images/`.
- A matching `ObjectiveImage` row is persisted with the relative file path.
- Deletion removes both the database row and the underlying file if it exists.

## Implementation caveats

- Several controller actions expose service or entity types directly instead of using dedicated request models.
- Some service methods return partially populated `ServiceResult<T>` objects even after successful writes.
- Startup and objective creation both trigger ML work, so changes to objectives can have broader runtime cost than a normal CRUD update.
