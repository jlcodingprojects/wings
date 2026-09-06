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
| 1A | Setup and control | Windows A/B prototype built and checked; physical-controller verification remains unconfirmed | General flight accepted; user authorised continued implementation |
| 1B | Flight feel | Dive, mode presentation and shared context-ready tuning implemented; 17 tests and visible player checks pass | User authorised continuing to 1C; detailed dive feedback remains open |
| 1C | Camera and contact | Assisted perching, swept contact, safe recovery and camera controls implemented | Landing rejected as VTOL-like; revised in 1D |
| 1D | Bird landing and takeoff | Forward approach/contact/departure, aim targeting and articulated rig implemented | Pending gameplay review |
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
- Subsequent feedback: user reports the general flight is great, can tune the parameters and authorises continuing the plan. This accepts the general flight foundation, without choosing a final A/B model or claiming a physical-controller test.

## Shared flight tuning decision — 2026-09-06

- User direction: flight parameters will later depend on the bird or flock; initially one setting is sufficient for all birds.
- Use the same shared defaults for all current birds. Retain A/B experiments for comparison; these are not separate species settings.
- Each simulation step consumes a resolved value snapshot. A neutral context preserves the existing behaviour; future bird/flock systems may supply influences without editing shared profile assets. Exact influences and their rules remain provisional.
- 1B adds bounded dive speed and simulation-driven presentation while preserving level-flight steering. No species differentiation or flock mechanics are introduced at this checkpoint.

## 1C review packet — 2026-09-06

- User authorised continuing from 1B; no final flight model or species/flock tuning was selected.
- Build: `0.3.0-checkpoint1c`, `Builds/Checkpoint1C/Wings.exe`.
- Delivered: adjustable camera look/recentering, swept contact and deflection, validated safe recovery, three temporary perches, assisted approach/cancellation/takeoff and contextual input.
- Evidence: zero build errors/warnings, 23 edit-mode tests and 19 standalone visible player checks pass. Menu, flight and perched captures inspected. See `Setup/checkpoint1c-evidence.json`.
- User feedback: landing looks like a VTOL aircraft. Research real footage, use aim-selected Land here, start perched, and support detailed wings/legs/head animation. 1D is explicitly revised to address this. Physical-controller verification remains open.

## 1D revision — 2026-09-06

- User requested bird-like landing/takeoff after rejecting 1C, accepting that this first revision may need more iteration.
- Deliver forward approach, flare, grasp/settle, leg push-off, aim-based Land here, and a visually composed perched opening.
- Keep high-fidelity wing, leg/toe, head/gaze and tail animation replaceable through an explicit contract. See `15_BIRD_MOTION_AND_RIG.md` and the editable Blender template.
- Build `0.4.0-checkpoint1d`; evidence and current limits in `CHECKPOINT_1D_REVIEW.md` and `Setup/checkpoint1d-evidence.json`.
- Decision: implementation ready for human review, not accepted realism or Stage 1 comfort.
- Revised order: **living flock flying (2B) follows this checkpoint**, before valley expansion (2A). The earlier 1D terrain/route scope is deferred. No other species/predator/content expansion is implied.

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
