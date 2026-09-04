# 01 — Game Specification

## Working title

**Wings of Wander**

## Genre

3D atmospheric exploration / flocking adventure.

No conventional combat. No weapons. No inventory-heavy survival mechanics.

## Player fantasy

"I am a bird. I can fly anywhere interesting, discover other birds, build a flock, and experience a beautiful changing world."

## Core loop

```text
Take flight
    ↓
Explore
    ↓
Spot something interesting
    ↓
Approach birds / landmarks / phenomena
    ↓
Recruit birds
    ↓
Grow flock
    ↓
Gain confidence and new flight capabilities
    ↓
Encounter environmental or predator threats
    ↓
Escape / regroup
    ↓
Discover further regions
```

## Controls

### Mouse

- Mouse movement: steer
- Hold left mouse: flap / gain altitude
- Release left mouse: glide
- Optional right mouse: camera look

### Controller

- Left stick: steer
- Right trigger: flap
- Release trigger: glide
- Right stick: camera look

No keyboard should be required for normal gameplay.

## Flight model

Three primary modes:

### Flap
- increases lift
- increases forward energy
- consumes stamina
- stronger wing animation

### Glide
- preserves momentum
- gradually loses altitude
- low stamina cost
- allows efficient long-distance travel

### Dive
- initiated by pitching downward
- rapidly increases speed
- temporarily increases manoeuvrability
- can be used to escape predators or bad weather

## Player resources

### Stamina

Represents physical flight energy.

Reduced by:
- repeated flapping
- fighting strong wind
- flying in unsuitable weather
- certain predator encounters

Recovered by:
- gliding
- resting at safe locations
- favourable wind
- landing/perching
- certain flock abilities

### Confidence

Represents the flock's emotional stability.

Reduced by:
- predator attacks
- sudden weather events
- near misses
- separation from flock
- flying into frightening conditions

Recovered by:
- regrouping
- flying with trusted flock members
- reaching safe landmarks
- favourable weather
- successfully escaping threats

Confidence should be a soft resource, not a traditional HP bar.

Low confidence causes:
- flock cohesion to weaken
- birds to scatter more
- recruitable birds to become harder to attract
- player flight to feel less stable through subtle camera/handling effects

## Threat philosophy

Predators should create:
- tension
- urgency
- flock disruption
- memorable escapes

They should NOT become conventional enemies with health bars.

A predator's success condition is:

> "Break up the flock or force the player to retreat."

## World

The world is a compact network of visually distinctive regions connected by natural flight corridors.

Example:

```text
                MOUNTAIN
                   |
        FOREST — HIGH VALLEY
          |          |
       RUINS — WATERFALL — LAKE
                     |
                  VILLAGE
                     |
                   COAST
                     |
                  ISLANDS
```

## Discovery

Interesting things are deliberately visible from a distance:
- waterfalls
- unusual rock formations
- ruins
- migrating flocks
- smoke
- storms
- glowing nocturnal locations
- seasonal phenomena

The game should rarely require a map marker.

## Success condition

The long-term objective is to assemble a diverse flock and eventually undertake a major migration route.

The game is complete when the player has:
- explored the major regions
- discovered most bird species
- learned how birds respond to the world
- built a large flock
- completed the Great Migration

## Failure

Avoid conventional death where possible.

If stamina reaches zero:
- forced glide/descent
- emergency landing

If confidence collapses:
- flock scatters
- some birds temporarily leave
- player must regroup

This keeps failure emotionally meaningful without being punishing.
