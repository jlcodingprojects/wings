using UnityEngine;

namespace Wings
{
    public sealed class BirdMotor : MonoBehaviour
    {
        public FlightProfile profile;
        public FlightInput input;
        public Transform visual, leftWing, rightWing;
        public FlightState State { get; private set; }
        public int Recoveries { get; private set; }
        public Vector3 origin = new Vector3(0, 18, -90);
        FlightState previous;
        float wingPhase;

        void Awake() { ResetFlight(); }

        public void ResetFlight()
        {
            State = new FlightState { position = origin, velocity = Vector3.forward * (profile != null ? profile.cruiseSpeed : 14) };
            previous = State;
            transform.SetPositionAndRotation(State.position, State.Rotation);
        }

        void FixedUpdate()
        {
            if (input == null || input.Paused || profile == null) return;
            previous = State;
            var next = FlightSimulation.Step(State, input.Intent, profile, Time.fixedDeltaTime);
            Vector3 travel = next.position - State.position;
            // Forgiving provisional contact. Assisted perching and refined deflection belong to 1C.
            if (travel.sqrMagnitude > 0 && Physics.SphereCast(State.position, 0.45f, travel.normalized, out var hit, travel.magnitude + 0.12f, ~0, QueryTriggerInteraction.Ignore))
            {
                next.position = State.position + travel.normalized * Mathf.Max(0, hit.distance - 0.12f) + hit.normal * 0.08f;
                next.velocity = Vector3.ProjectOnPlane(next.velocity, hit.normal);
            }
            if (next.position.y < 2 || next.position.y > 180 || Mathf.Abs(next.position.x) > 340 || Mathf.Abs(next.position.z) > 340)
            {
                Recoveries++;
                ResetFlight();
                return;
            }
            State = next;
        }

        void Update()
        {
            float alpha = input != null && input.Paused ? 1 : Mathf.Clamp01((Time.time - Time.fixedTime) / Time.fixedDeltaTime);
            transform.SetPositionAndRotation(Vector3.Lerp(previous.position, State.position, alpha), Quaternion.Slerp(previous.Rotation, State.Rotation, alpha));
            if (visual != null) visual.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(previous.bank, State.bank, alpha));
            if (input != null && !input.Paused) wingPhase += Time.deltaTime * (input.Intent.flap > 0.1f ? 13 : 2);
            float sweep = Mathf.Sin(wingPhase) * (input != null && input.Intent.flap > 0.1f ? 27 : 4);
            if (leftWing != null) leftWing.localRotation = Quaternion.Euler(0, 0, -sweep);
            if (rightWing != null) rightWing.localRotation = Quaternion.Euler(0, 0, sweep);
        }
    }
}
