# Wings of Wander — Plan Pack

A production-ready planning pack for a small, AI-assisted 3D exploration game about flight, flocking, discovery, recruitment, weather, seasons, time of day, and non-combat aerial threats.

## Design pillars

1. **Flight first** — movement is simple, expressive and satisfying.
2. **The flock is the protagonist** — birds behave as a coherent social organism.
3. **Discovery over objectives** — landmarks and interesting birds pull the player through the world.
4. **Threat without combat** — predators create tension by disrupting the flock rather than being fought.
5. **A living world** — time, weather and seasons materially change flight and bird behaviour.
6. **Painterly 3D** — low/medium-poly geometry combined with watercolour, gouache, ink and painted materials.
7. **AI-assisted production** — AI accelerates implementation and content production, while humans retain creative and technical authority.

## Pack contents

- `01_GAME_SPEC.md` — core game specification and gameplay loop
- `02_FLOCK_AND_BIRD_AI.md` — flock simulation, bird personalities, weather responses and predator AI
- `03_WORLD_TIME_WEATHER_SEASONS.md` — world simulation specification
- `04_PROGRESSION_AND_CONTENT.md` — birds, collection, progression and world content
- `05_TECHNICAL_ARCHITECTURE.md` — Unity/C# architecture and data model
- `06_AI_CODING_WORKFLOW.md` — AI-driven development workflow and agent rules
- `07_DEVELOPMENT_ROADMAP.md` — staged development plan and vertical slice
- `08_TASK_LIST.md` — ordered implementation backlog
- `09_ART_AND_AUDIO.md` — visual and audio direction
- `10_BALANCING_AND_TELEMETRY.md` — tuning framework
- `11_HUMAN_READABLE_SUMMARY.md` — concise project brief

## Recommended MVP

Build one compact region with:
- 1 player bird
- 8 recruitable bird species
- 2 predator types
- 4 times of day
- 4 seasons
- 4 weather states
- 6–10 landmarks
- 1 major storm event
- flock size up to ~30 simulated birds

Do not build a huge open world until the flight + flock + recruitment + threat loop is fun.
