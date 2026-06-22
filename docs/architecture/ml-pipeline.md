# ML Pipeline

## Purpose

The ML implementation groups objectives into clusters and uses those clusters to generate recommendation candidates and precomputed neighbor links.

## Entry points

The ML workflow is triggered from:

- application startup in `Program.cs`
- `POST /api/ML/train`
- objective creation in `ObjectiveService.CreateObjectiveAsync()`
- default objective insertion at startup if new objectives were added

## Input data

`MLService` builds clustering points from `Objective` rows using:

- `Location.X`
- `Location.Y`
- average review rating
- parsed numeric price from `Pret`
- objective type id

## Configuration

The service reads these values from `MachineLearning` configuration:

- `KMin`
- `KMax`
- `MaxIterations`
- `ReferencePointLongitude`
- `ReferencePointLatitude`
- `LocationWeight`
- `TypeWeight`
- `RatingWeight`
- `PriceWeight`

## Training flow

1. Load objectives from the database.
2. Skip retraining if the process already trained a model and the objective count has not changed.
3. Normalize coordinates, rating, price, and type using min-max scaling multiplied by feature weights.
4. Evaluate `k` from `KMin` to `KMax` using a custom elbow calculation based on WCSS.
5. Run a custom K-Means implementation with random centroid initialization.
6. Persist the chosen cluster id back onto each objective.
7. Rebuild `ClusterCentroids`.
8. Save diagnostic JSON output to `ml_output/elbow_data.json` and `ml_output/cluster_data.json`.

## Recommendation flow

`GetRecommendedObjectivesAsync(objectiveId, count)`:

1. Load the selected objective.
2. Determine nearby clusters based on centroid distance.
3. Load candidate objectives from those clusters.
4. Compute a weighted similarity score using location, rating, price, and type.
5. Sort by similarity descending and distance ascending.
6. Return a compact DTO with id, name, city, first image URL, average rating, and distance.

## Neighbor precomputation

`UpdateClusterNeighborsAsync()`:

1. Load clustered objectives with locations.
2. Load cluster centroids.
3. Clear the `ClusterNeighbors` table.
4. For each objective, find nearby clusters using centroid proximity.
5. Compute pairwise distance and similarity against other objectives in those clusters.
6. Persist the result so `/api/ML/neighbors/{objectiveId}` can read it directly.

## Output artifacts

- `ClusterId` on `Objective`
- `ClusterCentroids` rows
- `ClusterNeighbors` rows
- `ml_output/elbow_data.json`
- `ml_output/cluster_data.json`
- `ml_output/clusters_normalized_xy.png`
- `ml_output/clusters_geographic.png`
- `ml_output/clusters_pca.png`

## Current limitations

- Retraining cache only checks objective count, not content changes, so edits to existing objectives do not invalidate the cache by themselves.
- K-Means uses random initialization instead of `kmeans++`, which can make results less stable between runs.
- `GetClusterAnalysisAsync()` is effectively a placeholder and currently returns an empty result structure.
- Review data is read indirectly through navigation properties in several places without explicit eager loading during training, so future changes to EF behavior should be tested carefully.
