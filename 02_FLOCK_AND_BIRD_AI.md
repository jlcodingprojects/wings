# 02 — Flock and Bird AI

## Structure

A FlockDirector owns membership, loose formation guidance, confidence and regrouping. Each active bird simulates its own position, velocity, flight mode, neighbour response and social state. Birds respond to one another as well as the player; they are not animated offsets attached to a leader. Keep authored BirdDefinition assets separate from runtime membership and behavioural state.

Start with ordinary C# and bounded neighbour queries. Steering combines target following, separation, alignment, cohesion and terrain/obstacle avoidance. Avoid full rigidbody interactions between companions. Profile before adopting Jobs/Burst or more elaborate simulation LOD.

The production cap is 16 companions plus the player. Stress-test 30 companions. Distant decorative birds are visual-only and cannot recruit or trigger predator encounters.

## Definitions and behaviour

Each BirdDefinition has a stable unique ID, display name, habitat/availability, speed and steering limits, silhouette/presentation references, call, sociality, curiosity, boldness and environment response settings. Validate missing references, IDs and invalid ranges.

Committed species:
- Sparrow: social introductory recruit in meadow and sheltered landmarks.
- Swallow: fast aerial motion around open slopes.
- Kingfisher: water-associated encounters at lake and waterfall.
- Heron: slower, calm motion near shore and wet ground.

Initially use one shared flight setting for all birds. Later flight parameters may depend on bird or flock context, as requested by the user; define and review those influences when implementing the living flock. These are behaviour/flight differences, not progression stat upgrades. Keep companions within a compatible speed envelope; formation catch-up assistance prevents slow species becoming permanently stranded.

## Recruitment

States: wandering → notice → investigate → parallel flight → joining → companion.

Calm nearby parallel flight builds interest. Use separate enter/leave distances and a grace period so small fluctuations do not reset progress. Telegraph interest with orientation, calls and formation behaviour; no recruit button is required.

A bird has one stable identity and cannot occupy two active membership slots. Keep species discoveries, known individual records and active flock membership separate. At capacity, preserve discoveries but do not silently replace an existing companion; another bird can join when a slot becomes available.

Active members can transition through companion → leaving → independent, and later investigate and rejoin. Leaving is a visible behaviour, not destruction or death. Use provisional social/habitat motives and hysteresis; review their pacing at 2B. Predator scattering is a separate state from voluntary departure. Reserve scattered members' slots during regrouping; known independent birds do not consume active slots. Joining/leaving events must be explicit and observable.

## Confidence and recovery

Confidence affects formation and temporary scattering only. Scattered companions retain their membership reservation and regroup through simulated recovery steering. Safe-perch reconstruction is a fallback for stranded agents, not the normal flock movement mechanism. Loading reconstructs active formation while preserving known identities and membership states.

Do not erase journal discoveries or delete birds as a consequence of predator impact. Voluntary departure changes active membership without erasing identity. Keep the camera stable; any recoverable injury effect on flight must be reviewed at 2D.

## Hawk predator

One type: patrol → assess → telegraph → approach/feint or impact → disengage → cooldown.

Initial constraints:
- No encounter during the first five minutes of a new game.
- At least three seconds of readable warning before the first disruptive pass.
- Encounter duration, from warning to disengagement, at most 15 seconds.
- At least three minutes between encounter end and another warning.
- Only one active encounter.
- Safe perches and shelter prevent new attacks and end pursuit.
- Never spawn directly in the flight path; always provide an escape route.
- Impact can break up the flock and injure the player bird. Every bird remains alive; no death state, lethal damage or permanent predator loss exists.
- Injury is recoverable. Provisionally use healthy → injured → recovering → healthy, retaining reliable controls and safe-perch recovery. Exact effects, duration and feedback are reviewed at 2D before they are treated as accepted mechanics.

Cooldown and onboarding protection must survive saving/loading. Season/time skipping cannot bypass the encounter cooldown. Gameplay timers use active play time, not accelerated world time.

## Validation

Test duplicate recruitment, capacity reservations, hysteresis, voluntary leaving/rejoining, mixed-speed following, neighbour response, obstacles, separation recovery, identity preservation, nonlethal injury recovery and all predator timing/protection rules. Seed scenarios for repeatable setup without claiming exact physics replay.
