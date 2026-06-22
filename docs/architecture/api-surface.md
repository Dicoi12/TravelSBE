# API Surface

## Authentication and user endpoints

`UserController`

- `POST /api/User/Login` - authenticate and return JWT
- `POST /api/User/SignUp` - register a user
- `POST /api/User/ChangePassword` - change password for the authenticated user
- `GET /api/User` - return the authenticated user profile

## Objective endpoints

`ObjectivesController`

- `GET /api/Objectives/GetObjectivesAsync` - paginated objective listing with optional text search
- `GET /api/Objectives/GetObjectivesForModel` - lightweight objective export for modeling/search use
- `GET /api/Objectives/ImportObjectives` - insert hard-coded default objectives if missing
- `POST /api/Objectives/GetLocalObjectives` - filter objectives by name, type, rating, and optional location distance
- `GET /api/Objectives/{id}` - get one objective with images and reviews
- `POST /api/Objectives/PostObjective` - create an objective
- `PUT /api/Objectives/UpdateObjective` - update an objective
- `DELETE /api/Objectives/{id}` - delete an objective

## Objective image endpoints

`ObjectiveImageController`

- `GET /api/ObjectiveImage/{objectiveId}` - list images for an objective
- `POST /api/ObjectiveImage/upload` - upload an image and attach it to an objective, event, or experience
- `DELETE /api/ObjectiveImage/delete` - delete an image by URL

## Objective type endpoints

`ObjectiveTypeController`

- `GET /api/ObjectiveType`
- `GET /api/ObjectiveType/{id}`
- `POST /api/ObjectiveType`
- `PUT /api/ObjectiveType/{id}`
- `DELETE /api/ObjectiveType/{id}`

## Review endpoints

`ReviewController`

- `GET /api/Review/objective/{objectiveId}` - reviews for one objective
- `POST /api/Review`
- `GET /api/Review/{id}`
- `DELETE /api/Review/{id}`

## Event endpoints

`EventController`

- `GET /api/Event/GetAllEventsAsync`
- `GET /api/Event/{id}`
- `GET /api/Event` - filter by city or exact coordinates
- `POST /api/Event`
- `PUT /api/Event/{id}`
- `DELETE /api/Event/{id}`

## Experience endpoints

`ExperienceController`

- `GET /api/Experience`
- `GET /api/Experience/{id}`
- `GET /api/Experience/search`
- `POST /api/Experience`
- `PUT /api/Experience`
- `DELETE /api/Experience/{id}`

## Itinerary endpoints

`ItineraryController`

- `GET /api/Itinerary/GetItineraryAsync` - list itineraries
- `POST /api/Itinerary/AddItineraryAsync`
- `PUT /api/Itinerary/UpdateItineraryAsync`
- `GET /api/Itinerary/me` - itineraries owned by or visible to current user
- `GET /api/Itinerary/{id}`
- `DELETE /api/Itinerary/{id}`

## Itinerary detail endpoints

`ItineraryDetailController`

- `POST /api/ItineraryDetail/AddItineraryDetailAsync`
- `PUT /api/ItineraryDetail/UpdateItineraryDetailAsync`
- `DELETE /api/ItineraryDetail/{id}`
- `GET /api/ItineraryDetail/{id}`
- `GET /api/ItineraryDetail/GetAllItineraryDetailsAsync`

## Machine learning endpoints

`MLController`

- `POST /api/ML/train` - retrain clustering model and rebuild neighbors
- `GET /api/ML/recommendations/{objectiveId}` - recommendation list for an objective
- `GET /api/ML/visualization` - raw data for cluster visualization
- `POST /api/ML/neighbors/update` - rebuild precomputed neighbors
- `GET /api/ML/neighbors/{objectiveId}` - nearest neighbors from precomputed table
- `GET /api/ML/cluster-analysis` - placeholder response, analysis is not implemented yet

## Current API shape notes

- Most endpoints require authentication because the controllers are annotated with `[Authorize]`.
- Login and signup are explicit exceptions via `[AllowAnonymous]`.
- Endpoint naming is mixed: some routes are REST-like, while others use RPC-style names such as `GetObjectivesAsync` or `AddItineraryAsync`.
- Several actions return `ServiceResult<T>` directly, so consumers should expect success flags and validation messages in response payloads.
