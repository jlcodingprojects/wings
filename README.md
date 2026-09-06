# Wings of Wander — Approved Plan Pack

A compact Windows game about flight and flock dynamics: initially fly a single bird, then guide and watch birds flying together, joining, leaving and regrouping. Built interactively with the player steering progress at frequent checkpoints.

## Status

The first Windows single-bird prototype offers switchable assisted and momentum flight experiments. Toolchain checks, 11 edit-mode tests and the visible player smoke test pass; menu and flight captures were inspected on 2026-09-06. Human review and physical-controller checks are pending. See the [checkpoint 1A playtest guide](CHECKPOINT_1A_REVIEW.md), [installation record](12_INSTALLATION_CHECKLIST.md) and [flight/flock context](14_CORE_FLIGHT_AND_FLOCK_CONTEXT.md). No flight model or visual style has been accepted.

## Committed build

- One connected valley: meadow, woodland, lake, waterfall and high ridge.
- Four recruitable species: sparrow, swallow, kingfisher and heron.
- Up to 16 companions; test 30 as performance headroom.
- One hawk predator creating regular, gentle, nonlethal tension.
- Predator impact can scatter the flock and cause recoverable player injury. No bird ever dies.
- Automatic day/night and seasons; clear/rain/snow with independent wind.
- Six landmarks, persistent journal, save/load and a short migration finale.
- A 20–30-minute discovery route, followed by continued free exploration.
- Controller-first, effortless flight with mouse support.
- Provisional soft gouache/watercolour direction, finalised in Stage 3.

## Exactly three stages

1. **Initial gameplay:** prove 3D flight, controls, camera, contact and perching.
2. **World building and content:** complete exploration, flock, world changes and progression.
3. **Finalise graphics style:** choose a demonstrated treatment and finish assets, presentation and performance.

Each stage contains review checkpoints. Pause for human feedback at each checkpoint; do not interpret silence as acceptance.

## Reading order

| Document | Purpose |
|---|---|
| [Human-readable summary](11_HUMAN_READABLE_SUMMARY.md) | Concise creative brief |
| [Game specification](01_GAME_SPEC.md) | Flight, controls and core loop |
| [Flock and bird AI](02_FLOCK_AND_BIRD_AI.md) | Companions, recruitment and hawk |
| [World simulation](03_WORLD_TIME_WEATHER_SEASONS.md) | Time, seasons, weather and shelter |
| [Progression and content](04_PROGRESSION_AND_CONTENT.md) | Valley, species, discoveries and finale |
| [Technical architecture](05_TECHNICAL_ARCHITECTURE.md) | Stack, boundaries and asset pipeline |
| [Development workflow](06_AI_CODING_WORKFLOW.md) | Implementation and checkpoint rules |
| [Roadmap](07_DEVELOPMENT_ROADMAP.md) | Three stages and exit gates |
| [Task list](08_TASK_LIST.md) | Checkpoint-linked implementation backlog |
| [Art and audio](09_ART_AND_AUDIO.md) | Style selection and presentation |
| [Balancing and validation](10_BALANCING_AND_TELEMETRY.md) | Tuning and required evidence |
| [Installation checklist](12_INSTALLATION_CHECKLIST.md) | Existing software, downloads and connection tests |
| [Decision and checkpoint log](13_DECISIONS_AND_CHECKPOINTS.md) | Accepted direction and pending reviews |
| [Core flight and flock context](14_CORE_FLIGHT_AND_FLOCK_CONTEXT.md) | Single-bird foundation and living flock implementation constraints |

Implementation order is governed by the roadmap; accepted changes are recorded in the decision log and propagated to affected specifications. Scope extensions require a new recorded decision.
