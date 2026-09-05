using UnityEngine;

namespace Wings
{
    public enum FlightExperiment { Assisted, Momentum }

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
        public Quaternion Rotation => Quaternion.Euler(-pitch, heading, 0);
    }

    // Experimental movement rules, intentionally independent of player input and scene objects.
    public static class FlightSimulation
    {
        public static FlightState Step(FlightState state, FlightIntent intent, FlightProfile profile, float dt)
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
            float speed = profile.cruiseSpeed + intent.flap * profile.flapBoost;
            Vector3 desiredVelocity = state.Rotation * Vector3.forward * speed;
            desiredVelocity.y -= profile.glideDescent * (1 - intent.flap);
            float velocityBlend = 1 - Mathf.Exp(-profile.velocityResponse * dt);
            state.velocity = Vector3.Lerp(state.velocity, desiredVelocity, velocityBlend);
            state.position += state.velocity * dt;
            return state;
        }
    }
}
