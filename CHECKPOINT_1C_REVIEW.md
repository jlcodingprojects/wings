# Checkpoint 1C — camera, contact and perching

Build `0.3.0-checkpoint1c`: [launch Wings](Builds/Checkpoint1C/Wings.exe).

Three temporary roosts now support assisted landing, cancellation and takeoff. A nearby perch must have a clear approach. Landing takes about 2.2 seconds; takeoff takes about 1.2 seconds. These timings and the straight assisted path are experiments for feedback, not final flight dynamics.

## Try it

1. Resume at the starting position. Press controller **A**, keyboard **E**, or click the **Land** button to approach Meadow roost. Press again to cancel, or let the bird settle.
2. Press the same action to take off. Fly towards Woodland or Lakeside roost and watch the button name the nearest reachable perch.
3. Try a tree or ground contact: the bird deflects. Water contact, becoming stuck or leaving the test boundary returns it to its last clear safe perch, with the starting roost as fallback. No death or injury is introduced here.
4. Orbit with the right stick or hold the right mouse button. Press the right stick, **C**, or **Recenter** to centre the camera. Pause to adjust look sensitivity, vertical inversion and automatic recenter speed.
5. Compare both flight experiments with **Y / Tab**. Flight tuning remains shared and session-local, prepared for later bird/flock context.

A blocked landing leaves you flying; a blocked takeoff leaves you resting. An approach obstructed after it starts cancels; an obstructed takeoff recovers safely. Resume with A does not also request a landing. Mouse clicks over the action button do not also flap.

## Review focus

Please try landing, cancelling and taking off, and report whether the assistance feels comfortable or takes too much control. Also check whether the camera returns behind you at a comfortable pace. The next checkpoint is the longer flight playground and comfort route; flock implementation remains in Stage 2.

The Windows build has zero build errors/warnings; all 23 edit-mode tests pass. The visible player check exercises virtual-controller landing/cancellation/takeoff, actual water contact, mouse fallback, UI input consumption, orbit and pause. Menu, flight and perched captures were inspected. Results are recorded in `Setup/checkpoint1c-evidence.json`.

Physical-controller feel and hardware hot-plug still need human verification; automated controller checks use a virtual device. Temporary geometry and bird art remain provisional.
