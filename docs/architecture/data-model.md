# Data Model

## Base conventions

- Most entities inherit `BaseAuditEntity`, which provides `CreatedAt` and `UpdatedAt`.
- The main database is PostgreSQL.
- Spatial coordinates for `User`, `Objective`, and `Event` are stored as `geography(point)`.

## Core entities

### User

- Stores `UserName`, `Email`, `Phone`, `Role`, password `Hash`, password `Salt`, and optional `Location`.
- Unique indexes exist on `UserName`, `Email`, and `Phone`.

### Objective

- Central tourism record with `Name`, `Description`, raw `Latitude`/`Longitude`, optional `Location`, `City`, `Type`, `Website`, `Interval`, `Pret`, `Duration`, and `ClusterId`.
- Has many `ObjectiveImage`.
- Has many `Review`.
- Links to one `ObjectiveType` through `Type`.

### ObjectiveType

- Lookup table with `Name` and `Description`.

### ObjectiveImage

- Stores file metadata for images reused by objectives, events, and experiences.
- Foreign keys include `IdObjective`, `IdEvent`, and `IdExperienta`.
- The `FilePath` field is the stored relative path used to build public URLs.

### Review

- Connects a `User` to an `Objective`.
- Stores `Raiting`, `Comment`, and `DatePosted`.
- Objective review rows are cascade-deleted when the parent objective is removed.

### Event

- Stores `Name`, `Description`, `Country`, `City`, date range, optional coordinates, optional `Location`, optional related objective, `WebSite`, and images.

### Experience

- Stores user-generated travel experience content such as title, description, coordinates, location text, rating, visibility, and images.

### Itinerary

- Header record with `Name`, `Description`, optional owning `User`, date range, and child `ItineraryDetails`.
- Child details are configured for cascade delete.

### ItineraryDetail

- Represents one stop in an itinerary.
- Can reference either an `Objective` or an `Event`.
- Stores `VisitOrder`, optional `Date`, and optional `EstimatedTime`.

### Question / Answer

- Discussion-style entities tied to users and optionally objectives or events.
- Present in the data model but not currently surfaced by dedicated controllers in this repository snapshot.

## ML support entities

### ClusterCentroid

- Stores one persisted centroid row per cluster.
- Includes cluster averages for coordinates, rating, price, type, objective count, and `LastUpdated`.
- Unique index on `ClusterId`.

### ClusterNeighbor

- Stores precomputed objective-to-objective neighbor relationships.
- Includes `SourceObjectiveId`, `TargetObjectiveId`, `ClusterId`, `Distance`, `SimilarityScore`, and `LastUpdated`.
- Unique index on `(SourceObjectiveId, TargetObjectiveId)`.
- Separate index on `ClusterId`.

## Relationship summary

- `Objective -> ObjectiveImages`: one-to-many
- `Objective -> Reviews`: one-to-many with cascade delete
- `Event -> ObjectiveImages`: one-to-many
- `Experience -> ObjectiveImages`: one-to-many with cascade delete configured from the experience side
- `Itinerary -> ItineraryDetails`: one-to-many with cascade delete
- `ClusterNeighbor -> Objective`: two foreign keys, source and target

## Storage notes

- The code stores both raw numeric latitude/longitude and geospatial `Point` values for some entities.
- `ObjectiveService.UpdateMissingLocationsAsync()` exists to backfill missing `Location` values from numeric coordinates.
- Image binaries are not stored in the database; only metadata and relative file paths are persisted.
