# 06 — AI Coding Workflow

## Goal

Use AI as an implementation accelerator while preserving a small, testable and understandable codebase.

## Operating model

```text
Human defines behaviour
        ↓
AI proposes implementation
        ↓
AI writes small change
        ↓
Automated tests
        ↓
Human plays it
        ↓
Human accepts/rejects feel
        ↓
Commit
```

## Rules for the coding agent

1. Never rewrite large systems without permission.
2. Prefer small commits.
3. Read existing code before modifying it.
4. Preserve public APIs unless explicitly changing them.
5. Add tests for deterministic logic.
6. Do not introduce packages without justification.
7. Do not optimise before profiling.
8. Keep gameplay values data-driven.
9. Never silently change design rules.
10. When uncertain, present 2–3 implementation options before making a structural decision.

## Task prompt template

```text
TASK:
Implement [specific feature].

CONTEXT:
[relevant existing systems]

DESIGN:
[behaviour that must happen]

CONSTRAINTS:
- Unity current LTS
- C#
- no new package unless necessary
- preserve existing public APIs
- data-driven values

ACCEPTANCE:
- [observable behaviour]
- [tests]
- [performance requirement]

DO NOT:
- refactor unrelated code
- add UI unless requested
- change unrelated behaviour
```

## Agent phases

### Phase A — Understand
Agent reads:
- README
- architecture
- relevant source files
- current tests

### Phase B — Plan
Agent outputs:
- files to change
- implementation approach
- risks
- tests

### Phase C — Implement
Make the smallest useful change.

### Phase D — Verify
Run:
- compile
- unit tests
- play-mode tests where appropriate
- static analysis

### Phase E — Review
Human reviews:
- code
- game feel
- visual behaviour
- scope creep

## AI-assisted art workflow

```text
Design brief
 ↓
AI concept generation
 ↓
Human art direction
 ↓
3D blockout
 ↓
AI texture/material exploration
 ↓
Human cleanup
 ↓
Unity shader treatment
```

## AI-assisted content workflow

For every new bird:

1. Generate concept sheet.
2. Select silhouette.
3. Define gameplay role.
4. Create BirdDefinition.
5. Implement/verify behaviour using existing systems.
6. Add journal art.
7. Add audio.
8. Test across 4 weather states × 4 times × 4 seasons.
9. Tune recruitment.
10. Commit.

## AI-assisted QA

Ask the agent to generate tests for:
- weather modifiers
- stamina calculations
- confidence changes
- recruitment scoring
- predator detection
- flock cohesion
- season/time availability

Use deterministic seeds.

## Branching

Recommended:

```text
main
  └── develop
       ├── feature/flight
       ├── feature/flock
       ├── feature/weather
       └── feature/predators
```

Keep features small and merge frequently.

## Definition of done

A task is not done because the code compiles.

It is done when:
- acceptance criteria pass
- tests pass
- profiler is acceptable
- behaviour is observable in-game
- no unrelated regressions
- documentation is updated where necessary
