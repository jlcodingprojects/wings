using UnityEngine;

namespace Wings
{
    [DefaultExecutionOrder(100)]
    public sealed class FlightCamera : MonoBehaviour
    {
        public BirdMotor bird;
        public FlightInput input;
        public float distance = 8;
        public float recenterRate = 1.2f;
        Vector3 dampVelocity;
        float orbitYaw, orbitPitch;
        bool snap = true;
        float lastLookTime;
        int recoveries;
        public void Recenter(bool immediate = false) { if (immediate) snap = true; orbitYaw = orbitPitch = 0; dampVelocity = Vector3.zero; }
        void LateUpdate()
        {
            if (bird == null || input == null) return;
            if (recoveries != bird.Recoveries) { recoveries = bird.Recoveries; Recenter(true); }
            Vector2 look = input.Look;
            orbitYaw = Mathf.Clamp(orbitYaw + look.x, -110, 110);
            orbitPitch = Mathf.Clamp(orbitPitch - look.y, -35, 45);
            if (input.LookHeld) lastLookTime = Time.unscaledTime;
            if (!input.LookHeld && !input.Paused && Time.unscaledTime - lastLookTime > 0.8f)
            {
                orbitYaw = Mathf.Lerp(orbitYaw, 0, 1 - Mathf.Exp(-Time.deltaTime * recenterRate));
                orbitPitch = Mathf.Lerp(orbitPitch, 0, 1 - Mathf.Exp(-Time.deltaTime * recenterRate));
            }
            Quaternion orbit = Quaternion.Euler(orbitPitch + 9, bird.transform.eulerAngles.y + orbitYaw, 0);
            Vector3 target = bird.transform.position + Vector3.up * 1.7f;
            Vector3 desired = target - orbit * Vector3.forward * distance;
            desired = ClearCameraPosition(target, desired);
            var smoothed = snap ? desired : Vector3.SmoothDamp(transform.position, desired, ref dampVelocity, 0.22f, Mathf.Infinity, Time.unscaledDeltaTime);
            transform.position = ClearCameraPosition(target, smoothed);
            Vector3 aim = bird.transform.position + Vector3.up * 0.8f;
            transform.rotation = Quaternion.LookRotation((aim - transform.position).normalized, Vector3.up);
            snap = false;
        }
        public static Vector3 ClearCameraPosition(Vector3 target, Vector3 desired)
        {
            Vector3 delta = desired - target;
            if (delta.sqrMagnitude < 0.0001f) return desired;
            return Physics.SphereCast(target, 0.3f, delta.normalized, out var hit, delta.magnitude, ~0, QueryTriggerInteraction.Ignore)
                ? target + delta.normalized * Mathf.Max(0, hit.distance - 0.08f) : desired;
        }
    }
}
