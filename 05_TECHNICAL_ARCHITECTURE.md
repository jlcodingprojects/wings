# 05 — Technical Architecture

## Stack

Unity 6.3 LTS, C#, URP, Input System, Cinemachine 3, Unity Test Framework, Unity UI and Unity Audio. Windows x64 with DirectX 11 and Mono initially. Blender 4.5 LTS supplies source meshes/rigs and exports. Git with LFS stores source and binary assets.

Install the latest available 6000.3 patch at setup and pin it in ProjectVersion.txt. Resolve compatible released packages and commit manifest/lock files; no preview dependencies or automatic upgrades. See [installation checklist](12_INSTALLATION_CHECKLIST.md).

Local builds/tests come first. Hosted CI, third-party editor bridges, DOTS/ECS, middleware audio and paid assets are not baseline dependencies.

## Runtime boundaries

| Producer → consumer | Contract |
|---|---|
| Input → flight | Normalised steer, flap, look and land/takeoff intent |
| Flight → camera/animation/flock | Pose, velocity and movement mode |
| Environment → birds/flight/presentation | Immutable local time, season, precipitation, wind and shelter sample |
| Flock director → agents | Stable identity, active/reserved membership, loose formation guidance, joining/leaving and recovery |
| Bird simulation → bird simulation | Position/velocity snapshots and bounded neighbour observations for mutual steering |
| Predator impact → bird condition | Nonlethal injury and recoverable flock disruption; presentation consumes condition events |
| Progression → persistence | Stable IDs, roster, discoveries, finale state and safe perch |
| Gameplay → presentation | Discovery, recruitment, threat and environment events |

Use ScriptableObjects for authored FlightProfile, BirdDefinition, EnvironmentProfile, LandmarkDefinition and PredatorProfile data. Runtime state must not mutate shared definition assets.

Use a 50 Hz fixed simulation step with interpolated presentation. The kinematic flight motor sweeps its collision volume, handles contacts and produces a separate visual bank target. Camera updates after visual movement. Maintain one clear owner of player motion.

Keep bootstrap, simulation and presentation separate. A single gameplay scene is sufficient; do not introduce world streaming or a general service framework prematurely.

## Save/load

Versioned JSON stores discovered species/landmark IDs, known bird identities, active/reserved/independent membership state, recoverable player injury state, finale state, environment clock/transitions, safe perch and predator protection/cooldown state. Store settings separately so a new game preserves preferences.

Write a temporary file, finish it, then replace the main save while retaining a backup. On unreadable/corrupt save, attempt the backup and report recovery. Preserve an unsupported newer save instead of overwriting it. If neither copy is usable, offer a fresh game.

Validate the safe perch and references on load; use the starting perch if needed. Reconstruct companion formation. Do not persist individual agent transforms. Rest-time acceleration does not advance active-play predator timers.

## Asset pipeline

Keep Blender source art outside the imported Assets tree. Export FBX mesh/animation, PNG textures and WAV audio. Establish one metre as the reference scale and verify forward/up orientation with a source-to-engine smoke test.

Preserve Unity .meta files. Track binary source/export assets with Git LFS before production. Ignore Library, Temp, Logs and generated local builds. Author scene/prefab changes with Unity editor APIs rather than handwritten serialised scene edits.

## Automation

Project-owned C# editor entrypoints create/validate project assets, run checks and produce Windows builds. Blender Python creates/exports assets and renders previews. Command-line execution is the baseline connection; native GUI automation has not been established.

Do not run batch and interactive Unity against the same project simultaneously. Local tests must produce exit codes and retained logs; rendered captures require graphics-enabled execution.

## Performance and reproducibility

Target 60 FPS at 1440p on the inspected Ryzen 7 5700X / RTX 4070 Ti / 32 GB PC, with 16 companions and weather. Test 30 companions for headroom. Profile repeatable routes and spikes before adding Jobs/Burst.

Seeds reproduce initial conditions and random choices, not bit-identical Unity physics. Headless tests do not prove visual quality or GPU performance.
