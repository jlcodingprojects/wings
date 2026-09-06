using UnityEngine;

namespace Wings
{
    public enum FlightExperiment { Assisted, Momentum }
    public enum FlightMode { Glide, Flap, Dive, Approach, Perched, Takeoff }

    [CreateAssetMenu(menuName = "Wings/Flight experiment")]
    public sealed class FlightProfile : ScriptableObject
    {
        public FlightExperiment experiment;
        [Range(6, 28)] public float cruiseSpeed = 14;
        [Range(0, 12)] public float flapBoost = 5;
        [Range(20, 120)] public float turnRate = 70;
        [Range(0.3f, 10)] public float response = 4;
        [Range(15, 65)] public float maxPitch = 40;
        [Range(0, 65)] public float maxBank = 35;
        [Range(0, 1.5f)] public float glideDescent = 0.35f;
        [Range(1, 12)] public float velocityResponse = 5;
        [Range(0, 15)] public float diveBoost = 8;
        [Range(3, 20)] public float diveStartAngle = 10;
    }

    // A neutral context is used for every bird today. Later bird/flock systems can
    // supply modest influences without changing shared authored assets or player input.
    public readonly struct FlightContext
    {
        public readonly float speedScale, turnScale;
        public FlightContext(float speedScale, float turnScale)
        {
            this.speedScale = Mathf.Clamp(speedScale, 0.5f, 1.5f);
            this.turnScale = Mathf.Clamp(turnScale, 0.5f, 1.5f);
        }
        public static FlightContext Shared => new FlightContext(1, 1);
    }

    public readonly struct FlightTuning
    {
        public readonly FlightExperiment experiment;
        public readonly float cruiseSpeed, flapBoost, turnRate, response, maxPitch, maxBank;
        public readonly float glideDescent, velocityResponse, diveBoost, diveStartAngle;
        public float MaximumSpeed => cruiseSpeed + flapBoost + diveBoost;
        public FlightTuning(FlightProfile profile, FlightContext context)
        {
            experiment = profile.experiment;
            float speedScale = context.speedScale > 0 ? context.speedScale : 1;
            float turnScale = context.turnScale > 0 ? context.turnScale : 1;
            cruiseSpeed = Mathf.Max(1, profile.cruiseSpeed) * speedScale;
            flapBoost = Mathf.Max(0, profile.flapBoost) * speedScale;
            turnRate = Mathf.Max(0, profile.turnRate) * turnScale;
            response = Mathf.Max(0.1f, profile.response);
            maxPitch = Mathf.Clamp(profile.maxPitch, 15, 65);
            maxBank = Mathf.Clamp(profile.maxBank, 0, 65);
            glideDescent = Mathf.Clamp(profile.glideDescent, 0, 1.5f);
            velocityResponse = Mathf.Max(0.1f, profile.velocityResponse);
            diveBoost = Mathf.Max(0, profile.diveBoost) * speedScale;
            diveStartAngle = Mathf.Clamp(profile.diveStartAngle, 1, maxPitch - 1);
        }
    }

    public struct FlightIntent
    {
        public Vector2 steer; // positive Y requests climb
        public float flap;
    }

    public struct FlightState
    {
        public Vector3 position;
        public Vector3 velocity;
        public float heading, pitch, bank, yawRate;
        public FlightMode mode;
        public float flapEffort, diveAmount;
        public Quaternion Rotation => Quaternion.Euler(-pitch, heading, 0);
    }

    // Experimental movement rules, intentionally independent of player input and scene objects.
    public static class FlightSimulation
    {
        public static FlightState Step(FlightState state, FlightIntent intent, FlightProfile profile, float dt)
            => Step(state, intent, new FlightTuning(profile, FlightContext.Shared), dt);

        public static FlightState Step(FlightState state, FlightIntent intent, FlightTuning profile, float dt)
        {
            if (dt <= 0) return state;
            intent.steer = Vector2.ClampMagnitude(intent.steer, 1);
            intent.flap = Mathf.Clamp01(intent.flap);
            float blend = 1 - Mathf.Exp(-profile.response * dt);
            float desiredBank = -intent.steer.x * profile.maxBank;
            state.bank = Mathf.Lerp(state.bank, desiredBank, blend);
            float desiredYaw = profile.experiment == FlightExperiment.Assisted
                ? intent.steer.x * profile.turnRate
                : -state.bank / Mathf.Max(1, profile.maxBank) * profile.turnRate;
            state.yawRate = Mathf.Lerp(state.yawRate, desiredYaw, blend);
            state.heading = Mathf.Repeat(state.heading + state.yawRate * dt, 360);
            state.pitch = Mathf.Lerp(state.pitch, intent.steer.y * profile.maxPitch, blend);
            state.flapEffort = intent.flap;
            state.diveAmount = Mathf.InverseLerp(profile.diveStartAngle, profile.maxPitch, -state.pitch);
            // A small exit threshold prevents mode flicker around the dive entry angle.
            bool diving = -state.pitch > profile.diveStartAngle || (state.mode == FlightMode.Dive && -state.pitch > profile.diveStartAngle - 2);
            state.mode = diving ? FlightMode.Dive : intent.flap > 0.1f ? FlightMode.Flap : FlightMode.Glide;
            float speed = profile.cruiseSpeed + intent.flap * profile.flapBoost + state.diveAmount * profile.diveBoost;
            Vector3 desiredVelocity = state.Rotation * Vector3.forward * speed;
            desiredVelocity.y -= profile.glideDescent * (1 - intent.flap);
            desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, profile.MaximumSpeed);
            float velocityBlend = 1 - Mathf.Exp(-profile.velocityResponse * dt);
            state.velocity = Vector3.Lerp(state.velocity, desiredVelocity, velocityBlend);
            state.velocity = Vector3.ClampMagnitude(state.velocity, profile.MaximumSpeed);
            state.position += state.velocity * dt;
            return state;
        }
    }
}
