# 06 — Interactive Development Workflow

## Authority and scope

The human steers game feel, scope and art direction. The implementation agent makes routine technical decisions within accepted scope. On 2026-09-05 the user confirmed tool installation and authorised subsequent setup, project creation, package resolution, tests and implementation. The earlier download-only restriction is superseded for this work. This does not bypass gameplay review checkpoints.

Do not lock in a flight model or core gameplay choice early. Keep the first assisted and momentum flight experiments switchable and their tuning editable. Obtain feedback on meaningful design choices using an actual playable comparison.

Work on the next checkpoint only. Do not implement all stages in one unattended pass. Pause for feedback at each checkpoint; silence is not approval.

## Increment workflow

1. Read the relevant specification, decision log, existing implementation and tests.
2. State the checkpoint and acceptance criteria.
3. Implement a coherent, reviewable increment, keeping tuning data-driven.
4. Compile and run relevant automated checks.
5. Inspect visual/runtime evidence and package a playable Windows build.
6. Present changes, evidence, known issues and a short playtest route.
7. Record human response as accepted, revise or cut.
8. Apply requested revisions before advancing.

Routine internal refactors and necessary fixes may proceed within the checkpoint. Changes to core mechanics, scope, packages or architecture must be justified and reflected in the plan. Do not silently reverse recorded decisions.

## Review packet

Each checkpoint includes:
- Build identifier and launch instructions.
- What changed and what the player should try.
- Relevant test results and known limitations.
- Visual evidence where it helps.
- The specific feel/layout/style decision needed from the player.

Never claim controller testing, frame rate, launch success or artistic approval without evidence. An unavailable tool is a documented blocker, not a reason to declare a checkpoint complete.

## Repository practice

Use short feature branches from main when implementation begins; no permanent develop branch is required. Keep changes small and preserve unrelated work. Track binary assets with LFS and preserve .meta files. Do not publish, upload or introduce paid dependencies without appropriate authorisation.

Local verification precedes hosted CI. Ensure the interactive editor has released the project before batch execution.

## Definition of done

A checkpoint is implemented when acceptance behaviour, relevant tests and a playable build exist. It is accepted only after human feedback. Keep these two states distinct in the log.

The plan pack is maintained as a consistent set: changes to counts, controls, progression or stages must update the summary, specification, roadmap and backlog together.
