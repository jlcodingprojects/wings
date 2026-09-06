# Checkpoint 1A — single-bird flight comparison

Build: `0.1.0-checkpoint1a`, Unity 6000.3.23f1. This is an experiment for review, not an accepted flight model or art treatment.

## Play

Run [Wings.exe](Builds/Checkpoint1A/Wings.exe). Keep the entire `Builds/Checkpoint1A` folder together; the executable needs its adjacent data and libraries. No Unity editor is required to play this local build. Start on the pause/tuning panel, then choose **Fly / Resume**.

| Action | Controller | Mouse / keyboard |
|---|---|---|
| Steer and climb/descent | Left stick | Move pointer around screen centre; WASD also works |
| Flap / glide | Hold / release RT | Hold / release left button; Space also works |
| Look around | Right stick | Hold right button and drag |
| Compare A/B | Y | Tab, or click a mode in the pause panel |
| Pause | Start | Escape or Pause / tune button |
| Resume | A or Start | Fly / Resume button or Escape |
| Reset bird | X | R or Reset bird button |
| Adjust tuning while paused | D-pad up/down selects; left/right adjusts | Sliders and inversion toggle |

Keep the mouse near the centre crosshair for neutral flight. The HUD consumes pointer input. The camera keeps a level horizon. Pause is automatic when the window loses focus. The player returns to the starting position at the outer/height boundary; collisions are provisional and the reset button is always available. Perching is not implemented yet.

## Five-minute comparison

1. Begin in **A / Assisted**. Fly towards the first open arch, make a gentle right turn and release steering. Try climbing while flapping, then gliding.
2. Reset the bird and switch to **B / Momentum**. Repeat those actions with the same cruise speed. Notice the slower bank build-up and how the turn and velocity settle after release.
3. Switch mid-flight with Tab/Y. There is no scene reload or teleport when switching.
4. Pause and change one setting at a time: cruise speed, turn rate, response, camera distance or steering sensitivity. Each mode retains its own flight settings for this session; the authored defaults stay unchanged. **Reset tuning** restores them.
5. Report which qualities you want to keep or change: steering response, weight through turns, climb/descent, speed and camera position. A mixture, or a different approach, is a valid result.

The arches are optional orientation guides, not a race or progression system. The larger game remains guiding and observing a living flock. No birds die; predator injury and flock breakup belong to later work, not this prototype.

## Evidence and limitations

- Unity project uses URP 17.3.0, Input System 1.20.0, Cinemachine 3.1.7, Test Framework 1.6.0 and Visual Studio integration 2.0.26. `Game/Packages/manifest.json` and the editor-resolved lock file record the actual dependency set.
- Windows x64 / Mono / Direct3D11 build succeeded with zero errors and zero build warnings.
- Eleven edit-mode tests passed: input consumption/inversion, directional movement, neutral recovery, distinct momentum behaviour, profile switching and fixed-step trajectory scheduling at 30/60/120 render rates. These tests do not establish real-world frame rate or flight comfort.
- Visible Windows player check passed on the RTX 4070 Ti on 2026-09-06: synthetic gamepad input, mouse fallback after removal, mode switching, HUD input consumption, pause and nonblank menu/flight captures. Both captures were visually inspected. The earlier hidden-window black captures were superseded by this run; the test now rejects blank captures.
- No physical controller was connected during the automated run. Physical controller feel, hot-plug and comfortable sustained flight require your review.
- Blender 4.5.13 exports and renders the rigged reference. Unity confirms one-metre bounds, correct forward/up orientation and deformation from an imported animation clip.
- Bird, scene, contact and camera are deliberately provisional. No flock simulation, predator, injury, save/load or assisted perching is implemented. Session tuning is not persisted between launches.
- The current camera is a small replaceable follow/orbit script; Cinemachine is resolved and available for later camera experiments. No camera architecture is considered settled.

Raw local logs, test XML, captures and JSON results live in `Artifacts` and are excluded from Git. A compact evidence summary is retained in `Setup/checkpoint1a-evidence.json`.

## Rebuild and checks

Close Unity before these commands. The wrapper refuses to start while any Unity editor process is present.

```powershell
# Run from the Wings repository root.
& 'C:\Program Files\Blender Foundation\Blender 4.5\blender.exe' --background --python Tools/blender_smoke.py -- (Get-Location).Path
./Tools/Invoke-Unity.ps1 -Method Wings.Editor.CheckpointBuilder.Build -LogName checkpoint1a-build
./Tools/Invoke-Unity.ps1 -Test -LogName editmode-tests
# Opens a game window, runs scripted checks and closes it automatically.
./Tools/Test-Player.ps1 -Visible
```

The generator uses Unity editor APIs to create/refresh the scene and generated assets. It preserves existing flight profile defaults between rebuilds. Do not run the generator after hand-editing the generated scene unless you intend to regenerate that scene; keep experimental source changes in scripts/profiles or duplicate the scene first.

Human decision: **pending**. The next step is feedback on this comparison, not automatic expansion into the flock or final flight model.
