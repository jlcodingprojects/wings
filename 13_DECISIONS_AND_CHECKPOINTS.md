# 13 — Decisions and Checkpoint Log

## Accepted planning decisions — 2026-09-05

| Decision | Record |
|---|---|
| Audience/platform | Windows personal build; commercial release deferred |
| Flight | Relaxed and effortless; no stamina, stalls or forced-descent economy |
| Input | Controller-first, mouse supported |
| Core concept | Keep flight, flock recruitment and exploration; revise overcomplicated systems |
| Threats | Regular gentle, nonlethal hawk encounters |
| World | One rich connected region, four recruitable species, six landmarks |
| Progression | Discoveries and a short migration finale; no ten-level stat ladder |
| Environment | Automatic seasons; independent wind and precipitation; perch time skipping |
| Stack | Unity 6.3 LTS, URP/C#, Blender 4.5 LTS; scripted file/CLI workflow |
| Visuals | Provisional gouache/watercolour; final selection at 3A |
| Collaboration | Pause after every meaningful increment |
| Current work | User installed tools and authorised project creation and subsequent setup/implementation; checkpoint 1A in progress |

The old eight-species/two-predator MVP, stamina, camera destabilisation, exclusive wind weather state and eleven-stage roadmap are superseded.

## Implementation clarification — 2026-09-05

- Start with the installation checklist. Download required items and provide manual steps. Do not install, run installers, modify workloads or activate licences on the user's behalf without checking first.
- Initial gameplay is flying one bird. The overall game is about guiding and watching a living flock, with individually simulated birds flying together, joining, leaving and regrouping.
- No bird ever dies. Predator impact can break up the flock and injure the player bird. This supersedes the previous no-injury rule.
- Separate known identities from active membership; voluntary departures are not death or erased progression. The previous permanent active-membership assumption is superseded.
- Recoverable injury details and social departure triggers are provisional until demonstrated and reviewed. The existing three-stage/checkpoint structure remains in force.
- Setup downloads completed: Unity 6000.3.23f1 and Blender 4.5.13 x64. Both match publisher checksums and have valid Authenticode signatures; see `Setup/download-verification.json`. Manual steps are in `Setup/INSTALL_STEPS.md`. No software was installed, and no checkpoint has been accepted or declared playable.

## Checkpoints

### Setup completion and flight experiment direction — 2026-09-05

- User confirmed installation at `C:\Program Files\Unity 6000.3.23f1` and `C:\Program Files\Blender Foundation\Blender 4.5` and authorised all subsequent setup/project steps.
- Unity launched with an eligible licence; Blender reports 4.5.13 LTS; Visual Studio 2022 Unity workload is detected.
- User explicitly selected **both assisted and momentum flight as switchable experiments**. Neither is accepted as the final flight model. Tuning, camera and art remain provisional.
- Scope of this increment: a single-bird comparison playground, reproducible toolchain checks and a Windows player. Living flock implementation waits for its review checkpoint.

| ID | Name | Implementation | Human decision |
|---|---|---|---|
| 1A | Setup and control | Windows A/B prototype built; toolchain, 11 tests and visible player checks pass; physical-controller check pending | Pending |
| 1B | Flight feel | Pending | Pending |
| 1C | Camera and contact | Pending | Pending |
| 1D | Flight playground | Pending | Pending |
| 2A | Valley exploration | Pending | Pending |
| 2B | Living flock | Pending | Pending |
| 2C | Changing world | Pending | Pending |
| 2D | Gentle tension | Pending | Pending |
| 2E | Complete content loop | Pending | Pending |
| 3A | Style selection | Pending | Pending |
| 3B | Representative finish | Pending | Pending |
| 3C | Full-world treatment | Pending | Pending |
| 3D | Final playable build | Pending | Pending |

## 1A review packet — 2026-09-06

- Build: `0.1.0-checkpoint1a`, `Builds/Checkpoint1A/Wings.exe`.
- Delivered: switchable assisted/momentum single-bird flight, editable session tuning, stable follow/orbit camera, mouse/controller input and a temporary comparison space.
- Evidence: zero build errors/warnings, 11 edit-mode passes, visible Windows player smoke pass, inspected menu/flight captures, Blender scale/orientation/animation validation. See `Setup/checkpoint1a-evidence.json` and `CHECKPOINT_1A_REVIEW.md`.
- Limitations: no physical controller connected during automated tests; no human flight-feel acceptance; no perching/flock/predator implementation. All movement and art remain provisional.
- Decision: pending user playtest feedback. No automatic advance to 1B or flock production.

## Review entry template (for subsequent checkpoints)

- Date / checkpoint / build:
- Delivered changes:
- Verification evidence:
- Known issues:
- Human feedback:
- Decision: accepted / revise / cut.
- Required revisions and affected documents:
- Next authorised checkpoint:

Do not mark a checkpoint accepted without actual feedback. Routine numerical tuning is provisional until its associated review. Scope changes must update all affected documents before subsequent implementation.
