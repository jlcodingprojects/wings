# 10 — Balancing and Validation

## Targets

Relaxed flight leads. No stamina calculations or forced-descent balancing remains.

Initial values:
- Day: 12 minutes; season: 24 minutes; year: 96 minutes.
- Weather transition: at least 30 seconds.
- Predator onboarding protection: five active-play minutes.
- Warning: at least three seconds before disruption.
- Encounter: at most 15 seconds including warning.
- Cooldown: at least three active-play minutes after disengagement.
- Companion cap: 16; stress-test 30.
- Complete discovery route: 20–30 minutes.
- Performance: 60 FPS at 1440p on inspected development PC.

Tune flight speed, steering response, dive limits, glide descent, camera distance/recenter rate, recruitment radius/dwell/grace periods, flock spacing and recovery time through the relevant checkpoints. Record accepted values in data assets and the review log.

## Automated and scenario validation

| System | Required scenarios |
|---|---|
| Flight | Comparable fixed-step behaviour at 30/60/120 render FPS; neutral recovery and dive limits |
| Contact | Thin obstacles, steep terrain, high-speed sweep, water and stuck recovery |
| Perching | Valid approach, cancellation, invalid target, obstruction and blocked takeoff |
| Recruitment | Duplicate prevention, capacity, distance hysteresis and temporary interruptions |
| Flock | Mutual neighbour response, mixed speeds, obstacle avoidance, voluntary leaving/rejoining, scattering, capacity reservations and preserved identities/discoveries |
| Predator | Warning/duration/cooldown, opening protection, shelter, reload protection, recoverable injury and no lethal outcome for any bird |
| Environment | Year wrap, rest actions, pause, transition reload and valid weather combinations |
| Saves | Normal reload, interrupted write, corrupt primary, backup and unknown newer version |
| UI/input | Complete controller-only and mouse-only routes, hot-plug and no double-consumed input |

Use focused automated tests for state/timing/persistence logic and play-mode tests for meaningful engine integration. Do not mirror trivial implementation details in tests.

## Human and graphics validation

At every review ask about the relevant behaviour: flight comfort, camera effort, landing predictability, flock appeal, recruitment clarity, weather variety, predator fairness or exploration curiosity.

For Stage 3 review all season/time combinations plus representative weather extremes and important pairings. Inspect motion, not only still screenshots.

Measure a repeatable graphics-enabled route with 16 companions and weather. Record frame-time distribution and conspicuous spikes, resolution, quality settings and machine. Averages alone and headless execution cannot establish smooth GPU performance.

## Diagnostics

Use local logs and profiling counters for build ID, scenario seed, flight time, recruitment, scattering, discoveries and encounter timing. No remote analytics service is part of this build. Diagnostic capture must not change simulation timing materially.

A failed acceptance check is resolved before advancing its checkpoint. Do not call a human-dependent gate passed on the basis of automated tests alone.
