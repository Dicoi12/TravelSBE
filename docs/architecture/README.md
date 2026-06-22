# Architecture Notes

This folder captures the current implementation of the `TravelSBE` backend so future work can start from the code as it exists now.

## Documents

- `application-overview.md` - high-level structure, runtime model, and startup sequence
- `request-flow.md` - how requests move through controllers, services, mapping, and persistence
- `data-model.md` - entities, key relationships, and storage conventions
- `api-surface.md` - current controller and endpoint inventory
- `ml-pipeline.md` - clustering and recommendation implementation details
- `../clustering-visualization.md` - normalized X/Y chart used to present clusters

## Scope

These notes describe the implementation that is currently in the repository, not an idealized target architecture. Where the code has important constraints or rough edges, those are called out explicitly so they are visible during maintenance.
