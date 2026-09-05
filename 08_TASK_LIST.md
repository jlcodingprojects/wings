# 08 — Checkpoint Backlog

The user accepted the general 1A flight foundation and authorised continued implementation. Checkpoint 1B is implemented and automatically verified, awaiting feedback on dive/recovery. Physical-controller verification remains unconfirmed. Complete and review one checkpoint at a time.

## Stage 1

### 1A — Setup and control
- [x] Complete download verification and prepare manual installation instructions.
- [x] User completes installation and authorises subsequent setup/project work.
- [x] Verify Unity 6000.3.23f1, Blender 4.5.13 LTS, licensing, export/import and automated player smoke tests.
- [x] User reports the general flight is great.
- [ ] Confirm physical-controller input/hot-plug; the user's tested input device is unspecified.
- [x] Create URP project and commit resolved package versions.
- [x] Configure Git ignore/LFS and source asset conventions.
- [x] Add bootstrap, placeholder bird, initial space and two switchable kinematic steering experiments.
- [x] Keep single-bird flight state independent of player input so flock agents can use the same movement concepts later.
- [x] Add controller-first input and mouse guide; build and launch Windows player.
- [x] Prepare review packet in CHECKPOINT_1A_REVIEW.md.
- [x] Receive positive flight feedback and authorisation to advance to 1B.

### 1B — Flight feel
- [x] Implement bounded flap/glide/dive and neutral recovery for review.
- [x] Separate motor state from visual banking and wing animation.
- [x] Expose tuning and compare fixed-step trajectories under 30/60/120 render schedules.
- [x] Keep shared defaults with neutral context and isolated resolved values for future bird/flock influences.
- [x] Prepare review packet in CHECKPOINT_1B_REVIEW.md.
- [ ] Receive feedback on dive/recovery before advancing to 1C.

### 1C — Camera and contact
- [ ] Add stable camera, recentering and sensitivity/inversion controls.
- [ ] Add swept collisions, deflection and safe recovery.
- [ ] Add assisted approach, cancellation, perching and takeoff.
- [ ] Verify both input paths and UI input consumption.
- [ ] Present review packet and await response.

### 1D — Flight playground
- [ ] Add slopes, narrow passage, water, obstacles and several perches.
- [ ] Provide 5–10-minute route and complete ten-minute comfort review.
- [ ] Record Stage 1 acceptance before proceeding.

## Stage 2

### 2A — Valley exploration
- [ ] Lay out five habitat areas and six landmarks with placeholder assets.
- [ ] Review scale, sightlines and travel time.

### 2B — Living flock
- [ ] Implement one species with individual flight state, loose formation and mutual bounded neighbour steering.
- [ ] Add joining/leaving/rejoining hysteresis, stable identities and active/reserved cap handling.
- [ ] Add scattering/recovery without permanent loss.
- [ ] Verify mixed-speed capability and 30-companion stress scenario.
- [ ] Review flock behaviour before expanding species.

### 2C — Changing world
- [ ] Add 12-minute day, 24-minute seasons and 96-minute year.
- [ ] Add perch rest to next dawn/season and pause behaviour.
- [ ] Add constrained precipitation, independent wind and 30-second transitions.
- [ ] Add shelter, functional environment feedback and reachability checks.
- [ ] Review world pacing.

### 2D — Gentle tension
- [ ] Implement single hawk and warning/encounter/cooldown constraints.
- [ ] Protect opening and safe perches; preserve protection through reload.
- [ ] Implement and review recoverable player injury from impact; no death for any bird.
- [ ] Verify regrouping, escape routes, injury recovery and reliable control; review any flight effects explicitly.
- [ ] Review fairness and tension.

### 2E — Complete loop
- [ ] Complete four species, six discoveries and journal.
- [ ] Add versioned saves, backup/recovery and settings persistence.
- [ ] Add finale unlock/route, continued exploration and functional audio.
- [ ] Run complete controller-only and mouse-only playthroughs.
- [ ] Record Stage 2 acceptance.

## Stage 3

### 3A — Style selection
- [ ] Produce three same-scene visual treatments and obtain selection.

### 3B — Representative finish
- [ ] Finish one bird and environment area; verify export/rig/material pipeline.
- [ ] Obtain approval of the production reference.

### 3C — Full-world treatment
- [ ] Finish player, four companion species, hawk and environment assets.
- [ ] Finish seasonal dressing, weather/time lighting, UI, calls and music.
- [ ] Review all season/time combinations and weather extremes.

### 3D — Final playable build
- [ ] Profile 1440p route with 16 companions/weather and resolve material spikes.
- [ ] Run full regression and save/recovery scenarios.
- [ ] Package local Windows build and obtain final acceptance.
