using UnityEngine;

namespace Wings
{
    [DefaultExecutionOrder(100)]
    public sealed class FlightCamera : MonoBehaviour
    {
        public BirdMotor bird;
        public FlightInput input;
        public float distance = 8;
        Vector3 dampVelocity;
        float orbitYaw, orbitPitch;
        bool snap = true;
        public void Recenter() { snap = true; orbitYaw = orbitPitch = 0; }
        void LateUpdate()
        {
            if (bird == null || input == null) return;
            Vector2 look = input.Look;
            orbitYaw = Mathf.Clamp(orbitYaw + look.x, -110, 110);
            orbitPitch = Mathf.Clamp(orbitPitch - look.y, -35, 45);
            if (look.sqrMagnitude < 0.0001f && !input.Paused)
            {
                orbitYaw = Mathf.Lerp(orbitYaw, 0, 1 - Mathf.Exp(-Time.deltaTime * 1.2f));
                orbitPitch = Mathf.Lerp(orbitPitch, 0, 1 - Mathf.Exp(-Time.deltaTime * 1.2f));
            }
            Quaternion orbit = Quaternion.Euler(orbitPitch + 9, bird.transform.eulerAngles.y + orbitYaw, 0);
            Vector3 target = bird.transform.position + Vector3.up * 1.7f;
            Vector3 desired = target - orbit * Vector3.forward * distance;
            if (Physics.Linecast(target, desired, out var hit, ~0, QueryTriggerInteraction.Ignore)) desired = hit.point + hit.normal * 0.4f;
            transform.position = snap ? desired : Vector3.SmoothDamp(transform.position, desired, ref dampVelocity, 0.22f, Mathf.Infinity, Time.unscaledDeltaTime);
            Vector3 aim = bird.transform.position + Vector3.up * 0.8f;
            transform.rotation = Quaternion.LookRotation((aim - transform.position).normalized, Vector3.up);
            snap = false;
        }
    }
}
