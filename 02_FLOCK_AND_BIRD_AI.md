# 02 — Flock and Bird AI

## Architecture

Use three layers.

```text
WORLD
  ↓
FLOCK DIRECTOR
  ↓
INDIVIDUAL BIRD AGENTS
```

The Flock Director determines:
- desired flock centre
- formation density
- threat response
- migration direction
- regroup behaviour

Individual agents determine:
- local steering
- personality
- weather response
- curiosity
- fear
- recruitment behaviour

## Boids layer

Use weighted:
- separation
- alignment
- cohesion
- target following
- obstacle avoidance
- terrain avoidance

Weights should be configurable per species.

## Bird data

Each species has:

```text
Species
BaseSpeed
MaxSpeed
Acceleration
TurnRate
Stamina
Confidence
PreferredAltitude
WeatherTolerance[4]
SeasonPreference[4]
TimePreference[4]
PredatorFear
Sociality
Curiosity
Boldness
RecruitmentDifficulty
FlockRole
```

## Flock roles

Possible roles:
- Leader
- Navigator
- Scout
- Social
- Rear guard
- Weather specialist
- Predator-sensitive

Roles should be emergent initially rather than explicitly assigned.

## Recruitment AI

Wild bird states:

```text
Wandering
  ↓
NoticePlayer
  ↓
Observe
  ↓
Investigate
  ↓
ParallelFlight
  ↓
Follow
  ↓
JoinFlock
```

Recruitment score can combine:

```text
interest =
    species.sociality
  + player_proximity_score
  + matching_weather_score
  + flock_size_bonus
  + landmark_interest
  - fear
  - predator_pressure
  - bad_weather_penalty
```

The player should never need to press a "recruit" button.

## Predator AI

Predator states:

```text
Patrol
  ↓
DetectFlock
  ↓
Assess
  ↓
Approach
  ↓
Threaten
  ↓
Strike / Feint
  ↓
Disengage
```

Predators should generally prefer intimidation and disruption.

### Predator effects

A successful pass can:
- reduce flock confidence
- temporarily scatter nearby birds
- drain player stamina
- force altitude changes
- cause vulnerable birds to flee
- cancel an ongoing recruitment

## Predator types

### Hawk
Fast, localised attacks.
- strongest against small flocks
- short engagements
- high burst threat

### Eagle
Large-area intimidation.
- slower
- creates strong fear
- can force the flock lower or higher

### Owl
Night predator.
- difficult to see
- stronger during dusk/night
- uses surprise

Future:
- falcon
- harrier
- fictional predator for late-game regions

## Predator fairness rules

Never:
- spawn directly in front of the player
- kill a bird without readable warning
- chain attacks indefinitely
- completely invalidate a weather strategy

Always:
- telegraph with silhouette/audio
- give an escape vector
- allow the player to use terrain
- allow the flock to regroup

## Weather reactions

Each bird has a four-state response to each weather type:

- Thrives
- Comfortable
- Struggles
- Cannot fly effectively

Example:

| Species | Rain | Snow | Sun | Wind |
|---|---|---|---|---|
| Swallow | Comfortable | Struggles | Thrives | Thrives |
| Heron | Thrives | Struggles | Comfortable | Struggles |
| Eagle | Comfortable | Comfortable | Thrives | Thrives |
| Kingfisher | Thrives | Struggles | Thrives | Struggles |
| Owl | Comfortable | Comfortable | Struggles | Comfortable |
| Seabird | Comfortable | Comfortable | Thrives | Thrives |

These are starting values, not final balance.

## Struggling behaviour

A struggling bird:
- flaps more frequently
- loses altitude
- has lower max speed
- uses more stamina
- drifts further from flock centre
- may seek shelter
- emits contextual audio

A severely struggling bird should trigger a readable visual cue rather than a UI warning.

## Species-specific behaviour

Examples:

### Eagle
Likes wind and high altitude.

### Swallow
Excellent in wind, poor in snow.

### Heron
Very good in rain, poor in strong wind.

### Owl
Strong at night, weaker in bright daylight.

### Kingfisher
Likes rain and rivers, dislikes snow.

### Seabird
Thrives in wind and coastal storms.

This makes collecting birds mechanically meaningful.
