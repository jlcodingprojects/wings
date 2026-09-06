# Wings of Wander — Human-Readable Summary

You begin by flying a single bird through a small, changing valley. Flight should feel relaxed and effortless. The game centres on flock and flight dynamics: guiding and watching individually simulated birds flying together, joining, leaving and regrouping.

The first complete game has one connected valley, four recruitable species, six landmarks and a short migration finale. You can keep exploring after the finale.

There is no stamina meter, conventional combat or death for any bird. A hawk can break up your flock and injure your bird. Injury is recoverable, and scattered birds can regroup; its precise effects will be reviewed during implementation. Birds can also leave naturally and return without losing their identity or your discoveries.

Time and seasons advance automatically: a day takes 12 minutes, a season 24 minutes and a year 96 minutes. Resting at a perch skips to dawn or the next season. Wind can accompany clear skies, rain or snow. Weather creates atmosphere and mild changes in the flock rather than survival pressure.

## How we will build it

1. **Initial gameplay:** a simple 3D flight playground, controller-first controls, mouse support, camera, landing and forgiving collisions. Flying must be enjoyable before adding content.
2. **World building and content:** shape the valley, recruit birds, add world changes and a hawk, then complete the journal, saves and migration route.
3. **Finalise graphics style:** compare three treatments, choose one and finish the birds, environments, lighting, UI and sound.

You receive a playable review after each meaningful increment. Progress pauses for your feedback. You decide what feels right and which visual direction to use.

## Tools

Unity 6.3 LTS and C# build the game, URP renders it, and Blender 4.5 LTS creates assets. The agent works through scripts, files, logs and generated previews; you judge the actual feel in playable builds.

The machine already has older Unity and Blender installations. The installation checklist explains the versions to add and how to verify the workflow.

## Current status

A first single-bird Windows prototype is ready for review, with assisted and momentum flight as switchable experiments and editable session tuning. Setup and automated checks pass; the player still needs to judge flight feel and physical-controller behaviour. No checkpoint is human-accepted, and no flight model or art treatment is settled. See [the playtest guide](CHECKPOINT_1A_REVIEW.md).
