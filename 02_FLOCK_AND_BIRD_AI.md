# 02 — Flock and Bird AI

## Structure

A FlockDirector owns membership, formation targets, confidence and regrouping. Bird agents execute local steering and animation. Keep authored BirdDefinition assets separate from runtime membership and behavioural state.

Start with ordinary C# and bounded neighbour queries. Steering combines target following, separation, alignment, cohesion and terrain/obstacle avoidance. Avoid full rigidbody interactions between companions. Profile before adopting Jobs/Burst or more elaborate simulation LOD.

The production cap is 16 companions plus the player. Stress-test 30 companions. Distant decorative birds are visual-only and cannot recruit or trigger predator encounters.

## Definitions and behaviour

Each BirdDefinition has a stable unique ID, display name, habitat/availability, speed and steering limits, silhouette/presentation references, call, sociality, curiosity, boldness and environment response settings. Validate missing references, IDs and invalid ranges.

Committed species:
- Sparrow: social introductory recruit in meadow and sheltered landmarks.
- Swallow: fast aerial motion around open slopes.
- Kingfisher: water-associated encounters at lake and waterfall.
- Heron: slower, calm motion near shore and wet ground.

Use behavioural differences, not statistical upgrades to the player's motor. Keep companions within a compatible speed envelope; formation catch-up assistance prevents slow species becoming permanently stranded.

## Recruitment

States: wandering → notice → investigate → parallel flight → joining → companion.

Calm nearby parallel flight builds interest. Use separate enter/leave distances and a grace period so small fluctuations do not reset progress. Telegraph interest with orientation, calls and formation behaviour; no recruit button is required.

A bird can enter the roster only once. Stable species discoveries and individual roster entries are separate. At capacity, preserve discoveries but do not silently replace an existing companion; further flock membership waits for a future capacity decision.

## Confidence and recovery

Confidence affects formation and temporary scattering only. Scattered companions remain roster members. They regroup through recovery steering or reappear with the flock at a safe perch. Loading reconstructs formation rather than restoring every bird position.

Do not erase journal discoveries, permanently remove companions, wobble the player's camera or penalise steering.

## Hawk predator

One type: patrol → assess → telegraph → approach/feint → disengage → cooldown.

Initial constraints:
- No encounter during the first five minutes of a new game.
- At least three seconds of readable warning before the first disruptive pass.
- Encounter duration, from warning to disengagement, at most 15 seconds.
- At least three minutes between encounter end and another warning.
- Only one active encounter.
- Safe perches and shelter prevent new attacks and end pursuit.
- Never spawn directly in the flight path; always provide an escape route.
- Effects are temporary flock scattering, calls and formation changes; no injury or death.

Cooldown and onboarding protection must survive saving/loading. Season/time skipping cannot bypass the encounter cooldown. Gameplay timers use active play time, not accelerated world time.

## Validation

Test duplicate recruitment, capacity, hysteresis, mixed-speed following, obstacles, separation recovery, roster preservation and all predator timing/protection rules. Seed scenarios for repeatable setup without claiming exact physics replay.
