# 03 — World, Time, Weather and Seasons

## Simulation model

The world has four independent dimensions:

```text
TIME OF DAY
SEASON
WEATHER
REGION
```

Their combination determines:
- lighting
- sky
- ambience
- available birds
- bird behaviour
- predator activity
- flight conditions
- landmark appearance
- VFX
- music

## Four times of day

### Dawn
- soft cool-to-warm transition
- high bird activity
- migration activity
- low predator visibility

### Day
- brightest period
- strongest visibility
- broadest exploration window

### Dusk
- warm light
- high social/bird activity
- nocturnal species begin appearing
- predators become more active

### Night
- moon/stars
- restricted visibility
- nocturnal birds
- owl predators
- special landmarks and discoveries

Do not implement a minute-by-minute clock initially. Use four discrete states with smooth transitions.

## Four seasons

### Spring
- flowers
- fresh foliage
- high bird activity
- many species breeding/nesting behaviours

### Summer
- lush landscape
- long daylight
- strong thermal flight
- warm colour palette

### Autumn
- foliage colour changes
- migration
- strong wind opportunities
- rare migratory birds

### Winter
- snow in appropriate regions
- shorter day
- sparse foliage
- difficult flight for some species

## Four weather states

### Sun
Baseline condition.
- normal stamina
- good visibility
- thermals can appear

### Rain
- reduced visibility
- wet/painterly surfaces
- some birds thrive
- some birds struggle
- river/waterfall visuals intensify

### Snow
- reduced visibility
- cold atmosphere
- some birds lose stamina quickly
- snow accumulation changes landmarks

### Wind
- strongest mechanical weather state
- creates lift and drift
- skilled players can exploit it
- some birds thrive
- others struggle significantly

## Weather transition

Use a state machine:

```text
Clear
  ↕
Rain
  ↕
Wind
  ↕
Snow
```

Transitions should be gradual and visually telegraphed.

Rare major events:
- storm front
- blizzard
- gale
- clearing after rain

These should not be independent permanent weather types. They are intensity variants.

## Mechanical weather model

Every weather state supplies:

```text
Visibility
WindStrength
WindDirection
LiftMultiplier
StaminaMultiplier
ConfidenceMultiplier
BirdResponseModifier
```

## Seasonal modifiers

Each season changes:
- probability of weather
- daylight length
- bird availability
- landscape material palette
- migration routes
- landmark state

## Day/night + weather examples

### Summer dawn + wind
Ideal for soaring birds.

### Winter night + snow
Extremely challenging.
Only specialised birds should perform comfortably.

### Autumn dusk + rain
Excellent atmosphere and migration encounters.

### Spring day + sun
Best onboarding conditions.

## Design rule

The simulation should produce different gameplay, not merely different visuals.

A weather change is successful only if the player notices:

> "My flock is behaving differently."

## Shelter

Certain landmarks provide shelter:
- caves
- cliffs
- trees
- ruins
- bridges

Shelter reduces weather penalties and helps recover confidence.

## Forecasting

The player should learn weather indirectly through:
- cloud formations
- wind animation
- bird behaviour
- distant rain curtains
- lighting
- audio

Optional late-game ability:
- experienced birds can predict nearby weather changes.

## Performance

Do not simulate expensive weather physics for every bird.

Use:
- shared weather context
- localised noise
- cheap steering modifiers
- LOD simulation

Only nearby birds need full individual behaviour.
