# 10 — Balancing and Telemetry

## Primary tuning variables

### Flight
- flap lift
- flap acceleration
- glide drag
- dive acceleration
- stamina drain
- stamina recovery

### Flock
- cohesion
- separation
- alignment
- max flock size
- regroup speed

### Recruitment
- curiosity
- sociality
- fear
- proximity
- required follow duration
- weather modifier

### Predator
- detection range
- approach speed
- threat duration
- strike frequency
- confidence damage
- scatter radius
- disengage distance

### Weather
- stamina multiplier
- confidence multiplier
- wind force
- visibility
- lift
- bird-specific modifiers

## Design target

A normal exploration period should feel safe.

A predator encounter should be:
- readable within 1–2 seconds
- survivable almost every time
- disruptive rather than lethal

A major weather event should:
- create a meaningful route choice
- reward species composition
- provide an opportunity for mastery

## Example encounter

Player has:
- 10 birds
- 70% stamina
- 85% confidence

A hawk approaches.

Expected sequence:
1. warning audio
2. hawk visible
3. flock compresses
4. confidence drops slightly
5. hawk feints
6. player turns toward shelter/wind
7. flock scatters briefly
8. player regroups
9. confidence recovers

The player should finish thinking:

> "That was close."

Not:

> "The game randomly killed me."

## Telemetry

If analytics are used, capture only gameplay metrics needed for balancing.

Useful events:
- flight duration
- average flock size
- bird recruitment attempts
- successful recruitment
- predator encounters
- predator escapes
- confidence collapse
- stamina collapse
- weather encountered
- weather-induced retreat
- landmarks visited

## Playtest questions

After each prototype:
1. Did you enjoy simply flying?
2. Did you notice bird weather differences?
3. Did predator encounters feel fair?
4. Did you understand why a bird was struggling?
5. Did recruiting birds feel earned?
6. Did you voluntarily explore?
7. Did you want to find one more bird?

The seventh question is the most important.
