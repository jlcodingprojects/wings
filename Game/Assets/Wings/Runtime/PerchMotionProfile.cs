using UnityEngine;

namespace Wings
{
    public enum BirdActionPhase { Flight, Approach, Flare, Touchdown, Perched, Crouch, PushOff, Departure }

    // Animation-facing contract. Motion owns timing/contact; animation never teleports the motor.
    public struct BirdActionState
    {
        public BirdActionPhase phase;
        public float progress, flare, crouch, legReach, wingFold, grip;
        public Vector3 gazePoint, leftFoot, rightFoot;
        public bool feetPlanted;
    }

    [CreateAssetMenu(menuName = "Wings/Perch motion profile")]
    public sealed class PerchMotionProfile : ScriptableObject
    {
        public float minimumApproach = 0.65f, maximumApproach = 4.5f;
        public float flareStart = 0.62f, settleSeconds = 0.32f;
        public float crouchSeconds = 0.24f, departureSeconds = 0.85f;
        public float pushSpeed = 5, pushUpSpeed = 3.2f;
    }

    // A velocity-matched approach: never stop at acquisition and then accelerate towards the perch.
    public readonly struct LandingTrajectory
    {
        public readonly Vector3 start, end, startTangent, endTangent;
        public readonly float duration;
        public LandingTrajectory(FlightState state, Vector3 destination, float minimum = 0.65f, float maximum = 4.5f)
        {
            start = state.position; end = destination;
            float distance = Vector3.Distance(start, end);
            duration = Mathf.Clamp(distance * 1.65f / Mathf.Max(5, state.velocity.magnitude), minimum, maximum);
            startTangent = state.velocity * duration;
            var forward = Vector3.ProjectOnPlane(end - start, Vector3.up).normalized;
            // Small forward/downward contact velocity, arrested by the feet during touchdown.
            endTangent = (forward * 0.65f + Vector3.down * 0.18f) * duration;
        }
        public Vector3 Position(float t)
        {
            t = Mathf.Clamp01(t); float t2 = t * t, t3 = t2 * t;
            return start + (t3-2*t2+t)*startTangent + (-2*t3+3*t2)*(end-start) + (t3-t2)*endTangent;
        }
        public Vector3 Velocity(float t)
        {
            t = Mathf.Clamp01(t); float t2 = t*t;
            return ((3*t2-4*t+1)*startTangent + (-6*t2+6*t)*(end-start) + (3*t2-2*t)*endTangent) / duration;
        }
        public bool IsClear()
        {
            var from = start;
            for (int i = 1; i <= 32; i++) { var to = Position(i / 32f); if (!FlightContact.PathClear(from, to)) return false; from = to; }
            return true;
        }
    }

    public static class PerchTargeting
    {
        public static bool CanLand(Perch perch, FlightState state)
        {
            if (perch == null || !perch.CanApproach(state.position)) return false;
            var delta = perch.Point - state.position;
            if (delta.magnitude < Mathf.Max(4,state.velocity.magnitude*0.5f) || Vector3.ProjectOnPlane(delta, Vector3.up).magnitude < delta.magnitude * 0.65f) return false;
            var direction = state.velocity.sqrMagnitude > 1 ? state.velocity.normalized : state.Rotation * Vector3.forward;
            return Vector3.Dot(direction, delta.normalized) > 0.65f;
        }
        public static Perch Select(Perch[] perches, FlightState state, Ray aim)
        {
            Perch selected = null; float best = Mathf.Cos(10 * Mathf.Deg2Rad);
            foreach (var perch in perches)
            {
                if (!CanLand(perch, state)) continue;
                float score = Vector3.Dot(aim.direction.normalized, (perch.Point - aim.origin).normalized);
                if (score <= best) continue;
                selected = perch; best = score;
            }
            return selected;
        }
    }
}
