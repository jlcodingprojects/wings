using UnityEngine;

namespace Wings
{
    public static class FlightContact
    {
        public const float Radius = 0.45f;
        const float Skin = 0.04f;
        public static bool IsClear(Vector3 point) => !Physics.CheckSphere(point, Radius, ~0, QueryTriggerInteraction.Ignore);
        public static bool PathClear(Vector3 from, Vector3 to)
        {
            var delta = to - from;
            return IsClear(from) && IsClear(to) && (delta.sqrMagnitude < 0.0001f || !Physics.SphereCast(from, Radius, delta.normalized, out _, delta.magnitude, ~0, QueryTriggerInteraction.Ignore));
        }

        public static FlightState Move(FlightState previous, FlightState desired, out bool contacted, out bool water)
        {
            contacted = water = false;
            var displacement = desired.position - previous.position;
            if (displacement.sqrMagnitude < 0.000001f) return desired;
            if (!Physics.SphereCast(previous.position, Radius, displacement.normalized, out var hit, displacement.magnitude + Skin, ~0, QueryTriggerInteraction.Ignore)) return desired;
            contacted = true;
            water = hit.collider.GetComponentInParent<WaterSurface>() != null;
            desired.position = previous.position + displacement.normalized * Mathf.Max(0, hit.distance - Skin);
            var incoming = desired.velocity;
            var tangent = Vector3.ProjectOnPlane(incoming, hit.normal);
            // Head-on contacts bounce away; glancing contacts slide. No damage or input penalty.
            var direction = tangent.sqrMagnitude > incoming.sqrMagnitude * 0.12f ? tangent.normalized : Vector3.Reflect(incoming, hit.normal).normalized;
            desired.velocity = direction * incoming.magnitude * 0.65f;
            if (desired.velocity.sqrMagnitude > 0.001f)
            {
                var flat = new Vector3(direction.x, 0, direction.z);
                if (flat.sqrMagnitude > 0.001f) desired.heading = Mathf.Atan2(flat.x, flat.z) * Mathf.Rad2Deg;
                desired.pitch = Mathf.Asin(Mathf.Clamp(direction.y, -1, 1)) * Mathf.Rad2Deg;
                desired.yawRate = 0;
            }
            return desired;
        }
    }
}
