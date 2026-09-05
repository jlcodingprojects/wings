# 01 — Game Specification

## Intent and scope

Working title: **Wings of Wander**. A single-player, offline Windows game about flight and flock dynamics. Initially the player flies one bird; the broader experience is guiding and watching a living flock, with individuals flying together, joining, leaving and regrouping while exploring a valley.

Flight is relaxed and effortless. Controller tuning leads; mouse gameplay remains supported. There is no death for any bird, conventional combat, inventory-heavy survival or stamina resource. Predator impact can break up the flock and injure the player bird. Injury is recoverable; its presentation and flight effects need a concrete review at 2D. Bird behaviours are simulated gameplay rules, not claims of biological accuracy.

Core loop: fly → notice a landmark or bird → approach → recruit or discover → explore with the flock → encounter gentle environmental/predator variety → regroup → continue. A short migration finale provides closure; free exploration remains available.

The first complete build is one valley, four recruitable species, one hawk predator, six landmarks and up to 16 companions. Multiple regions and dozens of species are expansion ideas.

## Flight

Use assisted forward flight with a fixed-step kinematic motor and swept collision movement. Separate movement, visual banking and camera behaviour.

- Left stick controls heading and climb/descent.
- Neutral steering eases toward comfortable forward flight.
- Right trigger flaps; release glides with gentle descent.
- Downward pitch increases speed within a bounded dive envelope.
- Turning remains predictable; diving does not grant arbitrary extra manoeuvrability.
- Flapping is always available. No stalls, stamina depletion or forced precision sequences.
- Mild wind drift is allowed in Stage 2; it cannot strand the player or invalidate flight assistance.

Expose tuning through a FlightProfile asset rather than hard-coded species-specific controller branches. Initially all birds use the same shared defaults. Later bird or flock context can influence resolved flight values without mutating the shared profile or adding player-controller special cases. The general flight foundation was positively reviewed on 2026-09-06; dive tuning and future context rules remain reviewable at their checkpoints.

## Controls and camera

| Action | Controller | Mouse |
|---|---|---|
| Steer | Left stick | Cursor displacement from visible flight guide |
| Flap | Right trigger | Hold left button |
| Glide | Release trigger | Release left button |
| Look | Right stick | Hold right button and drag |
| Land / take off | A | Click contextual land/takeoff control |
| Menu | Menu/Start | Click persistent menu control |
| Menu navigation | D-pad/stick, A confirm, B back | Pointer and click |

UI interaction consumes its input; clicking a UI control must not also flap. During mouse camera drag, suspend mouse steering changes and ease toward neutral flight. Controller hot-plug/disconnection must not leave an action held; display mouse fallback instructions.

Keep a stable horizon by default, shake disabled, smooth recentering and adjustable steering/look sensitivity and inversion. Settings must be reachable by controller or mouse. Confidence never changes player input or camera stability.

## Landing and recovery

A landing request is accepted only near a valid, unblocked perch. Assistance completes the approach; another landing request cancels an approach and resumes flight. Invalid requests leave flight unchanged with brief feedback.

A while perched initiates assisted takeoff into clear space. Water contact or an unrecoverable obstruction returns the player to the last safe perch without lost progress. Ordinary collisions slow and deflect the bird. Validate the recovery destination before use; retain the starting perch as a fallback.

## Resources and failure

No stamina bar or exhaustion mechanic. Flock confidence is internal behavioural state: lower confidence widens spacing and triggers temporary scattering. Journal discoveries and known bird identities persist while active flock membership can change. A bird leaving the flock remains alive and can return.

Predators create readable warnings, brief approaches and recoverable separation; impact can injure the player bird. No injury progresses to death, and no predator permanently deletes companions. Provisionally retain reliable steering and safe-perch recovery; injury tuning is not yet accepted.

## Acceptance

Stage 1 is accepted only after a comfortable ten-minute playtest demonstrating flight, camera, landing and forgiving recovery. Content must not be used to mask unsatisfying flight.
