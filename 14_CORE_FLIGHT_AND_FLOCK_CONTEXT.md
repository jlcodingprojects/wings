# Core flight and flock implementation context

User direction recorded on 2026-09-05. This context guides the work after installation; it is not a claim of implemented gameplay or permission to skip review checkpoints.

## Experience

Initially fly one bird. The larger game is guiding and watching a flock: birds move together, react to neighbours and terrain, join, leave, scatter and regroup. Every active companion has its own simulated movement and social state. Loose formation guidance supports these interactions without fixing birds to decorative offsets.

The current scope stays at 16 companions, with 30 tested for headroom. This means a complete behavioural simulation for active flock birds, not a new commitment to aerodynamic CFD, every bird in the valley, or unlimited population simulation.

## Single-bird foundation

At 1A/1B, separate human input from flight intent, simulation state and presentation. The fixed-step motor owns movement; animation and the camera consume its output. Capture position, velocity, heading and flap/glide/dive/perch mode. Player and future AI intent can use the same movement concepts with authored species limits.

On 2026-09-06 the user approved the general flight foundation and clarified that one shared flight setting is enough initially. Bird/flock-dependent parameters come later. Resolve shared settings into per-step value snapshots; keep context neutral today and avoid committing a species/flock aggregation policy ahead of review.

Prove comfortable steering, flight, camera and contact with the single bird first. Do not fabricate package locks or engine test results before the chosen editor is installed and the project exists.

## Living flock

At 2B, agents observe a consistent neighbour snapshot and combine separation, alignment, cohesion, local avoidance and loose social targets. The director manages membership and recovery without directly placing agents each frame. Observe the flock while flying straight, banking, climbing, changing pace and passing obstacles.

Keep three concepts separate: discovered species; stable known bird identities; current active or scattered/reserved membership. An independent bird can investigate, join, leave and later rejoin under the same identity. Departure is visible and motivated; exact motives and timing are provisional. A scattered member retains a slot during regrouping. Never silently evict a bird when full.

## Predator impact and injury

No bird ever dies, including companions and predators. Impact can break up the flock and injure the player bird. There is no lethal damage threshold, corpse, game-over state or permanent predator deletion of a companion.

At 2D, demonstrate recoverable injury and safe recovery. Healthy → injured → recovering → healthy is a proposed state model. Duration, cues and any flight-envelope effects need review; do not quietly introduce stamina, loss of steering, forced crashes or camera wobble. Preserve injury/recovery state through save/load once saves exist.

## Review evidence

- Single bird: controllable Windows build, both input paths, comfortable ten-minute flight route.
- Flock: visible mutual reactions, coherent turns, mixed-speed following, joining/leaving/rejoining, bounded crowding and natural regrouping.
- Predator: warning and cooldown constraints, flock breakup, recoverable injury, safe-perch escape and no lethal outcomes.

User revision after 1C: checkpoint 1D replaces VTOL-like landing with bird-like approach/contact/takeoff, aim-selected perches and an articulated rig. **Living flock flying (2B) is the next checkpoint after 1D review**, ahead of broader valley expansion (2A). Injury remains in 2D. The motion/action contract is shared with future flock agents; general flight settings remain shared initially.
