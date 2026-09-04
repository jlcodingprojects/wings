# 05 — Technical Architecture

## Recommended stack

### Engine
Unity, current LTS release.

### Language
C#.

### Rendering
URP with custom painterly shader stack.

### 3D
Blender.

### Audio
FMOD or Unity Audio for MVP.

### Version control
Git.

### CI
GitHub Actions.

### AI coding
GitHub Copilot / coding agent + ChatGPT.

## Core systems

```text
GameBootstrap
├── WorldSimulation
│   ├── TimeOfDaySystem
│   ├── SeasonSystem
│   └── WeatherSystem
│
├── FlightSystem
│   ├── PlayerFlightController
│   ├── FlightPhysics
│   └── FlightCamera
│
├── FlockSystem
│   ├── FlockDirector
│   ├── BoidSimulation
│   └── BirdAgent
│
├── WildlifeSystem
│   ├── BirdSpawner
│   ├── PredatorDirector
│   └── RecruitmentSystem
│
├── WorldSystem
│   ├── Region
│   ├── Landmark
│   └── Discovery
│
├── ProgressionSystem
│   ├── PlayerProgression
│   └── BirdCollection
│
└── Presentation
    ├── PainterlyRenderer
    ├── VFX
    ├── Audio
    └── UI
```

## Data-driven design

Use ScriptableObjects for:
- BirdDefinition
- WeatherDefinition
- SeasonDefinition
- RegionDefinition
- LandmarkDefinition
- PredatorDefinition
- ProgressionDefinition

Avoid hard-coding species behaviour.

## BirdDefinition

```csharp
public class BirdDefinition : ScriptableObject
{
    public string Id;
    public float MaxSpeed;
    public float Acceleration;
    public float TurnRate;
    public float BaseStamina;
    public float BaseConfidence;

    public float Sociality;
    public float Curiosity;
    public float Fear;
    public float Boldness;

    public WeatherResponse[] WeatherResponses;
    public TimePreference TimePreference;
    public SeasonPreference SeasonPreference;
}
```

## World simulation

Expose one immutable snapshot:

```text
WorldState
{
    TimeOfDay
    Season
    Weather
    WindDirection
    WindStrength
    Visibility
}
```

Birds consume this context rather than querying multiple managers.

## Simulation LOD

### Near
Full bird AI + animation + collision.

### Mid
Simplified flock steering.

### Far
GPU/instanced visual flock or aggregate representation.

### Very far
Animated silhouettes / particles.

## Determinism

Use seeded random streams where possible so:
- bugs are reproducible
- AI tests are stable
- world behaviour can be replayed

## Save data

Persist:
- discovered birds
- discovered landmarks
- progression level
- flock roster
- journal state
- unlocked regions

Do not persist every individual bird position.

## Performance target

Initial target:
- 60 FPS
- stable controller input
- 30 fully simulated nearby birds
- hundreds of distant visual birds

Profile before introducing DOTS/ECS.

Only move flock simulation to Jobs/Burst/ECS if profiling demonstrates a need.
