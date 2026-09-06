# Checkpoint 1D — bird landing and takeoff

Build `0.4.0-checkpoint1d`: [launch Wings](Builds/Checkpoint1D/Wings.exe).

You now start perched on a branch overlooking the lake. Press **A / E** or click **Take off**. The bird crouches, opens its wings, pushes away from the branch and accelerates into flight.

Point towards a perch. Mouse aiming uses the pointer; controller aiming uses the flight direction. **Land here** appears on the selected perch when it is within reach and there is room for a forward approach. Click it or press **A / E**. The bird approaches, brakes nose-up, reaches its feet, grasps and settles. Press again before contact to cancel. A perch behind you, directly beneath you, blocked or already too close at speed is not offered.

The nearest practice bough is ahead of the opening branch. Try landing on it, taking off, circling back and cancelling an approach. **R / X** resets to the opening perch. **Tab / Y** still switches flight experiments; pause to adjust the shared flight defaults. Use right mouse drag/right stick to inspect the bird and **C / right-stick press** to recenter.

The temporary bird now demonstrates articulated wings, tail, legs/toes and head look. Its geometry and poses remain deliberately provisional. The [motion and rig contract](15_BIRD_MOTION_AND_RIG.md) records footage references, ownership, the editable Blender skeleton and how detailed animations can replace the prototype.

Review whether the forward approach, flare, foot contact and push-off read as a bird, and whether aiming makes the correct perch easy to choose. This is a first revision, not a claim of finished realism. The next checkpoint is **flock flying**, before broader valley exploration.

Verification: zero build errors/warnings, 27 edit-mode tests and 27 player checks passed. Phase captures were inspected. The Blender export contains 34 bones and 43 weighted mesh parts; Unity verifies animated wing, knee, head and tail channels. Evidence is retained in `Setup/checkpoint1d-evidence.json`. Physical-controller feel and sustained comfort require human review.
