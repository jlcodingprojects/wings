# Checkpoint 1B — flap, glide and dive

Build `0.2.0-checkpoint1b`: [launch Wings](Builds/Checkpoint1B/Wings.exe). The previous accepted foundation remains available in `Builds/Checkpoint1A`.

## What changed

Level-flight steering retains the same defaults you liked. Pitching down beyond ten degrees gradually adds dive speed, up to an eight m/s bonus at full downward pitch. The overall speed limit includes cruise, flap and dive contributions. Dive speed does not increase turn authority. Releasing steering levels the bird and sheds the extra speed; flapping stays available throughout.

The HUD names the current mode. Wing animation now consumes simulated flap effort and dive state, with a gentle wing tuck when diving without flapping. Visual presentation is separate from the movement component.

All current birds use a shared default profile with neutral context. Future bird/flock systems can supply context influences to a resolved value snapshot without modifying shared defaults. No species differences, flock aggregation policy or stat progression has been implemented or settled. The A/B modes remain experiments, not species-specific settings.

## Try it

1. Resume flight, climb to roughly 60–80 metres, then release flap and pitch downward briefly. Watch the mode and speed readout.
2. Centre steering and flap to recover. Compare the recovery in Assisted and Momentum using Tab or controller Y.
3. Pause and adjust **Dive speed boost** and **Flap speed boost**. Setting dive boost to zero restores the previous speed behaviour. Existing speed, response and camera controls remain available.

Mouse/controller mappings are unchanged from [1A](CHECKPOINT_1A_REVIEW.md). D-pad tuning includes the new boost settings. Tuning remains session-local. Report whether dive acceleration and recovery feel comfortable; the previous positive general-flight feedback is retained.

## Verification

- Windows x64 / Mono / Direct3D11 build: zero errors and warnings.
- Seventeen edit-mode tests passed, including dive limits, unchanged turn authority, recovery, context isolation and flap/dive/recovery sequences under 30/60/120 render schedules with a 50 Hz simulation. This is trajectory comparison, not a measured frame-rate guarantee.
- Visible Windows player smoke test passed: synthetic controller input, dive acceleration/limit, flap recovery, mode switching, mouse fallback, HUD input consumption, pause and rendered captures. Menu and flight captures inspected.
- No physical gamepad was connected to the automated run; hardware feel/hot-plug is still unconfirmed.
- Evidence: `Setup/checkpoint1b-evidence.json`; raw logs and captures: `Artifacts`.

Review of this increment is pending. The next planned checkpoint is 1C: camera/contact refinement and assisted landing, perching and takeoff. The living flock remains a later checkpoint; no death or predator changes are introduced here.
