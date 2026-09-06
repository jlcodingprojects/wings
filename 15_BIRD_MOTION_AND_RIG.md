# Bird motion and rig contract — checkpoint 1D

The 1C landing was rejected as VTOL-like. This revision is an adjustable bird-motion experiment, not final biomechanics or production animation.

## References inspected

- [BBC Earth: Slow Motion Pigeon Flight](https://www.youtube.com/watch?v=uAIZrRAR31Y), especially about 0:20–0:25: feet support the bird as wings open, then the bird leaves with active wingbeats. Frames inspected in the browser.
- [Cornell Lab: Capturing Epic Slow Motion Footage of Backyard Birds](https://www.youtube.com/watch?v=oZvA2eL_Sho), about 2:18, 2:23 and 2:28: the titmouse approaches the feeder, changes wing/body posture and reaches its feet forward for contact. Frames inspected in the browser.
- [Berg and Biewener, 2010: Wing and body kinematics of takeoff and landing flight in the pigeon](https://journals.biologists.com/jeb/article/213/10/1651/9685/Wing-and-body-kinematics-of-takeoff-and-landing): takeoff and landing differ in body/stroke-plane orientation; landing reduces forward speed. The paper links high-speed supplementary movies; those movie URLs did not play in the browser, so direct visual review used the BBC/Cornell footage.

These are motion references only. No external footage or artwork is bundled with the game. Numerical timing and angles below are game experiments, not values measured from those videos or universal species rules.

## Movement and targeting

Mouse pointing projects a ray from the camera; controller selection follows the intended flight direction. A target must lie in the aim cone, within reach, in front of incoming flight, with forward braking room and a clear approach. The contextual **Land here** button belongs to that exact perch. Pointer movement into its button retains that target while it remains reachable. A full curved-path clearance check runs again when committing.

Landing uses a Hermite trajectory with the incoming velocity as its start derivative and a small forward contact velocity. The bird does not stop when assistance starts. The movement root follows the trajectory while the body separately pitches up for braking, the tail fans, and feet reach forward. Contact plants the feet; the body compresses and wings fold while the root stays at the perch. An approach can be cancelled before foot contact; contact/settling is a short committed phase.

Takeoff has a brief supported crouch and wing opening, a leg push that immediately moves forward/up, then an accelerating powered departure. It hands its velocity back to free flight. A blocked departure remains perched; obstruction after push-off triggers validated safe recovery.

Normal startup and reset place the bird on the overlook branch. The current shared flight profile remains unchanged. `PerchMotionProfile` holds provisional approach, flare, settle, crouch and departure timings separately from general flight settings.

## Ownership

| Component | Responsibility |
|---|---|
| `FlightSimulation` / `FlightTuning` | Existing shared A/B flight, with neutral bird/flock context |
| `BirdMotor` | World position, velocity, action timing, target/contact validity and recovery |
| `LandingTrajectory` / `PerchTargeting` | Testable approach geometry and aim selection |
| `BirdActionState` | Phase, normalized progress, flare, crouch, leg reach, wing fold, grip, gaze and world foot contacts |
| `BirdPresentation` | Temporary procedural posing and current movement-intent head look |
| `BirdRig` | Explicit replaceable skeleton bindings and optional Generic Animator |

Future flock agents can call `RequestLanding(perch)` and use the same movement/action contract without mouse/controller dependencies. Species artwork and animations must not write the motor's world position.

## Prototype assets and replacement

- `Game/Assets/Wings/Generated/BirdPrototype.prefab` contains the current transform rig and primitive geometry. It has body/neck/head, shoulder/elbow/wrist chains, tail feather pivots, hip/knee/ankle/foot chains and separate toes. This is a visible articulation prototype, not the final mesh.
- `SourceArt/Bird/BirdRigTemplate.blend` is an editable armature and rigidly weighted mesh template using matching bone names. `Game/Assets/Wings/Art/Bird/BirdRigTemplate.fbx` verifies the import path; it is not the model used in the current player scene.
- `Tools/create_bird_rig.py` rebuilds that source/export from `SourceArt/Bird/PrototypeRig.json`, emitted by the Unity scene builder. Run scene creation first, Blender export second, then the player build. Rebuilding is intended for this generated template; production artwork should live in separate authored files.
- Units: metres; Unity +Z forward/+Y up, Blender -Y forward/+Z up. Preserve bone names or explicitly rebind `BirdRig`; final proportions and additional feather/face joints remain open.

For a detailed FBX, use a Generic Animator with root motion disabled. Enable `BirdRig.useAuthoredAnimation` to stop procedural bone writes. The existing adapter supplies `ActionPhase` (int), `ActionProgress`, `FlapEffort`, `Flare` and `Speed` (floats) when matching parameters exist. An authored controller may blend or time-warp approach, flare, grasp, settle, perch, crouch, push and departure clips. Bone axes of an imported armature can differ from the procedural rig; authored clips own those axes rather than reusing prototype Euler rotations blindly.

`BirdPresentation.Pose` exposes world foot targets and gaze for a later IK/animation layer. Detailed contact IK, feather deformation, head stabilization, blink/face motion, per-species gait and polished transitions still require production work. The present Blender check verifies that wing, knee, head and tail animation channels survive export/import; it does not claim that a final controller or high-fidelity skin has been produced.

## Review

Check forward approach, braking, foot reach/grasp, push-off and the aim/button interaction at normal speed. Adjusting this prototype after feedback is expected. Flock flying follows this checkpoint per the user's revised order; broader valley exploration is deferred until after the flock experiment.
