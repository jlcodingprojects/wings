# 03 — World, Time, Weather and Seasons

## Independent dimensions

Time, season, precipitation and wind are separate inputs. Rain can be windy; snow can be calm. Do not retain the old sun/rain/snow/wind exclusive state machine.

An environment service provides a local immutable sample containing time phase, season blend, precipitation type/intensity, wind vector, visibility and shelter influence. Presentation and birds consume it; they do not coordinate by querying unrelated global managers.

## Clock

- One full day/night cycle: 12 minutes of active gameplay.
- One season: two cycles (24 minutes).
- One year: four seasons (96 minutes).
- Order: spring → summer → autumn → winter → spring.
- Begin at spring dawn.
- Dawn/day/dusk/night are presentation phases of continuous cycle progress.
- Pause/menu time does not advance simulation.

At a safe perch, provide controller/mouse actions to advance to next dawn or next season. Resting settles transitions safely and does not count as real elapsed time for predator cooldowns. Save world clock and transition state.

All availability conditions needed for progression must be reachable by resting; no full-year wait is required.

## Weather and wind

Precipitation presets: clear, rain, snow. Independent wind has direction and strength. Region and season constrain legal combinations; snow belongs to winter and appropriate high ground.

Transitions last at least 30 seconds during normal gameplay and are signalled through clouds, lighting, ambience and distant precipitation. Rest transitions may use a short fade to establish the target context without showing an accelerated storm.

Weather affects ambience, formation, shelter-seeking and mild drift. It cannot require a specific species, force stamina failure or block access to essential destinations. Begin with comfortable clear conditions.

No expensive per-bird weather physics. Share regional context and sample local shelter cheaply.

## Seasons and shelter

Stage 2 uses simple seasonal palette/dressing swaps and functional weather visuals. Stage 3 supplies coherent foliage, ground cover, water/sky treatment and weather effects.

Shelter landmarks reduce weather influence and enable calm regrouping. Landing and resting must remain usable in every legal environment combination.

## Acceptance

Test automatic year wrap, each rest action, save/load during transitions, sheltered versus exposed samples and valid precipitation combinations. Review all 16 season/time combinations and representative weather/wind extremes. Validate the actual configured content availability, not an arbitrary exhaustive 4×4×4 matrix.
