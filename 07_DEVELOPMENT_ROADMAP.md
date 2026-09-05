# 07 — Three-Stage Development Roadmap

Implementation begins only after the documentation pass. Follow checkpoints in order and pause for human feedback at each.

## Stage 1 — Initial gameplay

Goal: flying a single bird is pleasant in a simple 3D landscape. Establish flight state and movement boundaries that will also support individually simulated flock birds. Setup is complete; the user accepted the general flight foundation and authorised continuing. All birds initially share flight defaults; later bird/flock influences remain reviewable.

| Checkpoint | Deliverable | Review |
|---|---|---|
| 1A Setup and control | Verified toolchain, Windows build, placeholder bird, basic steering and both input paths | Can launch and steer; controller-first direction |
| 1B Flight feel | Assisted flap/glide/dive, neutral recovery and data-driven tuning | Ease, speed, turn response and enjoyment |
| 1C Camera and contact | Stable camera, settings, swept collisions, assisted perching/takeoff and safe recovery | Comfort, landing predictability and forgiveness |
| 1D Flight playground | Slopes, water, passage, obstacles, several perches and a 5–10-minute route | Spatial scale and sustained flight enjoyment |

Exit: human enjoys ten minutes of flight; both input paths and camera work comfortably; landing and recovery are forgiving. No content expansion until accepted.

## Stage 2 — World building and content

Goal: complete discovery-to-finale gameplay with placeholder art.

| Checkpoint | Deliverable | Review |
|---|---|---|
| 2A Valley exploration | Connected valley and six functional landmarks | Layout, travel time and sightlines |
| 2B Living flock | One species proves mutual flight dynamics, joining/leaving, loose formation and regrouping | Believability and satisfaction when guiding and observing |
| 2C Changing world | Automatic time/seasons, rest, weather and independent wind | Pacing, readability and comfort |
| 2D Gentle tension | One hawk, warnings, timing constraints, flock breakup and recoverable player injury; no deaths | Fairness, recovery and tension |
| 2E Complete loop | Four species, persistent journal/roster, save/load, functional audio and migration finale | 20–30-minute route and replayability |

Exit: complete loop works, progress survives reloads, content is reachable without waiting a year, and human accepts pacing. Final asset production waits.

## Stage 3 — Finalise graphics style

Goal: coherent presentation without changing accepted gameplay.

| Checkpoint | Deliverable | Review |
|---|---|---|
| 3A Style selection | Three treatments of the same scene, lighting and camera route | Select one direction |
| 3B Representative finish | One finished area and bird | Quality, readability and production reference |
| 3C Full-world treatment | All birds, valley assets, seasons/weather/time, UI and audio | Coherence and readability in motion |
| 3D Final playable build | Performance tuning and full regression review | Final acceptance |

Exit: selected style is applied coherently, flight remains comfortable, full loop passes and the Windows build meets the measured performance target.

No fixed calendar delivery promise. Revisions at checkpoints determine actual pace; estimates can follow measured implementation throughput.
