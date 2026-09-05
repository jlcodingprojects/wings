using UnityEngine;

namespace Wings
{
    public sealed class BirdMotor : MonoBehaviour
    {
        public enum Activity { Flying, Approach, Perched, Takeoff }
        public FlightProfile profile;
        public FlightInput input;
        public Perch[] perches = new Perch[0];
        public Perch startingPerch;
        public Perch LastSafePerch { get; private set; }
        public Activity CurrentActivity { get; private set; }
        public float approachSeconds = 2.2f, takeoffSeconds = 1.2f;
        public FlightContext Context { get; set; } = FlightContext.Shared;
        public FlightTuning Tuning => new FlightTuning(profile, Context);
        public FlightState RenderState { get; private set; }
        public FlightState State { get; private set; }
        public int Recoveries { get; private set; }
        public Vector3 origin = new Vector3(0, 18, -90);
        FlightState previous;
        FlightState assistStart;
        Perch targetPerch;
        float assistTime, stuckTime, feedbackUntil;
        string feedback;
        public string Status => Time.unscaledTime < feedbackUntil ? feedback : CurrentActivity == Activity.Perched && LastSafePerch != null ? "Resting at " + LastSafePerch.displayName : "A / E or click: land, cancel approach, or take off";
        public Perch NearbyPerch
        {
            get
            {
                Perch nearest = null; float distance = float.MaxValue;
                foreach (var perch in perches)
                {
                    if (perch == null || !perch.CanApproach(State.position)) continue;
                    float candidate = Vector3.SqrMagnitude(perch.Point - State.position);
                    if (candidate < distance) { distance = candidate; nearest = perch; }
                }
                return nearest;
            }
        }
        public string ActionLabel => CurrentActivity == Activity.Approach ? "Cancel landing" : CurrentActivity == Activity.Perched ? "Take off" : CurrentActivity == Activity.Takeoff ? "Taking off…" : NearbyPerch != null ? "Land · " + NearbyPerch.displayName : "Land · find a clear perch";

        void Awake() { ResetFlight(); }

        public void ResetFlight()
        {
            CurrentActivity = Activity.Flying; targetPerch = null; stuckTime = 0; feedbackUntil = 0;
            State = new FlightState { position = origin, velocity = Vector3.forward * (profile != null ? Tuning.cruiseSpeed : 14) };
            previous = State;
            transform.SetPositionAndRotation(State.position, State.Rotation);
        }

        void FixedUpdate()
        {
            if (input == null || input.Paused || profile == null) return;
            previous = State;
            if (CurrentActivity == Activity.Perched)
            {
                if (LastSafePerch == null || !LastSafePerch.IsUsable) Recover("Perch obstructed; moved to safety.");
                return;
            }
            if (CurrentActivity != Activity.Flying) { AdvanceAssistance(Time.fixedDeltaTime); return; }
            var next = FlightSimulation.Step(State, input.Intent, Tuning, Time.fixedDeltaTime);
            next = FlightContact.Move(State, next, out bool contacted, out bool water);
            if (water) { Recover("Water contact — back to your safe perch."); return; }
            bool obstructed = !FlightContact.IsClear(State.position) || (contacted && Vector3.Distance(next.position, State.position) < 0.025f);
            stuckTime = obstructed ? stuckTime + Time.fixedDeltaTime : 0;
            if (stuckTime > 1.5f) { Recover("Obstruction — back to your safe perch."); return; }
            if (next.position.y < -5 || next.position.y > 180 || Mathf.Abs(next.position.x) > 340 || Mathf.Abs(next.position.z) > 340)
            {
                Recover("Valley boundary — back to your safe perch.");
                return;
            }
            State = next;
        }

        public void RequestPerchAction()
        {
            if (CurrentActivity == Activity.Approach) { CancelApproach(); return; }
            if (CurrentActivity == Activity.Takeoff) { Notify("Takeoff is already in progress."); return; }
            if (CurrentActivity == Activity.Perched)
            {
                if (LastSafePerch == null || !LastSafePerch.CanTakeOff) { Notify("Takeoff path blocked. Try again when clear."); return; }
                targetPerch = LastSafePerch; BeginAssistance(Activity.Takeoff); return;
            }
            var candidate = NearbyPerch;
            if (candidate == null) { Notify("Move closer to a perch with a clear approach."); return; }
            targetPerch = candidate; BeginAssistance(Activity.Approach);
        }
        void BeginAssistance(Activity activity)
        {
            CurrentActivity = activity; assistStart = State; assistTime = 0;
            Notify(activity == Activity.Approach ? "Approaching " + targetPerch.displayName + " — press again to cancel." : "Taking off from " + targetPerch.displayName);
        }
        public void CancelApproach()
        {
            if (CurrentActivity != Activity.Approach) return;
            CurrentActivity = Activity.Flying; targetPerch = null;
            var state = State; state.mode = FlightMode.Glide; state.velocity = state.Rotation * Vector3.forward * Tuning.cruiseSpeed;
            State = state; Notify("Landing cancelled — free flight.");
        }
        public void AdvanceAssistance(float dt)
        {
            bool landing = CurrentActivity == Activity.Approach;
            if (!landing && CurrentActivity != Activity.Takeoff) return;
            if (targetPerch == null || !targetPerch.IsUsable)
            {
                if (landing) { CancelApproach(); Notify("Perch is no longer clear — landing cancelled."); }
                else Recover("Takeoff obstructed — moved to safety.");
                return;
            }
            assistTime += dt;
            float t = Mathf.Clamp01(assistTime / Mathf.Max(0.2f, landing ? approachSeconds : takeoffSeconds));
            Vector3 destination = landing ? targetPerch.Point : targetPerch.LaunchPoint;
            Vector3 position = Vector3.Lerp(assistStart.position, destination, Mathf.SmoothStep(0, 1, t));
            if (!FlightContact.PathClear(State.position, position))
            {
                if (landing) { CancelApproach(); Notify("Approach blocked — landing cancelled."); }
                else Recover("Takeoff blocked — back to safety.");
                return;
            }
            var next = State;
            next.position = position;
            next.velocity = (position - State.position) / Mathf.Max(0.0001f, dt);
            next.heading = Mathf.LerpAngle(assistStart.heading, targetPerch.transform.eulerAngles.y, t);
            next.pitch = Mathf.Lerp(assistStart.pitch, landing ? 0 : 18, t);
            next.bank = Mathf.Lerp(assistStart.bank, 0, t); next.yawRate = 0; next.diveAmount = 0;
            next.flapEffort = landing ? 0.3f : 1;
            next.mode = landing ? FlightMode.Approach : FlightMode.Takeoff;
            State = next;
            if (t < 1) return;
            if (landing) SetPerched(targetPerch);
            else
            {
                CurrentActivity = Activity.Flying; targetPerch = null;
                next.mode = FlightMode.Flap; next.velocity = next.Rotation * Vector3.forward * Tuning.cruiseSpeed; State = next;
                Notify("Airborne — you're in control.");
            }
        }
        void SetPerched(Perch perch)
        {
            LastSafePerch = perch; targetPerch = null; CurrentActivity = Activity.Perched; stuckTime = 0;
            State = new FlightState { position = perch.Point, heading = perch.transform.eulerAngles.y, mode = FlightMode.Perched };
            previous = State;
            Notify("Perched at " + perch.displayName + ". Press A / E or Take off when ready.");
        }
        public void Recover(string reason)
        {
            Recoveries++; stuckTime = 0;
            if (LastSafePerch != null && LastSafePerch.IsUsable) SetPerched(LastSafePerch);
            else if (startingPerch != null && startingPerch.IsUsable) SetPerched(startingPerch);
            else
            {
                // Validate a clear emergency position rather than teleporting into a blocked perch.
                Vector3 safe = origin;
                int tries = 0;
                while (!FlightContact.IsClear(safe) && tries++ < 60) safe += Vector3.up * 3;
                if (!FlightContact.IsClear(safe)) { input.Paused = true; Notify("No clear recovery point. Pause and reset the scene."); return; }
                ResetFlight();
                var state = State; state.position = safe; State = previous = state;
            }
            Notify(reason);
        }
        void Notify(string message) { feedback = message; feedbackUntil = Time.unscaledTime + 4; }

        void Update()
        {
            float alpha = input != null && input.Paused ? 1 : Mathf.Clamp01((Time.time - Time.fixedTime) / Time.fixedDeltaTime);
            transform.SetPositionAndRotation(Vector3.Lerp(previous.position, State.position, alpha), Quaternion.Slerp(previous.Rotation, State.Rotation, alpha));
            var rendered = State;
            rendered.bank = Mathf.Lerp(previous.bank, State.bank, alpha);
            rendered.flapEffort = Mathf.Lerp(previous.flapEffort, State.flapEffort, alpha);
            rendered.diveAmount = Mathf.Lerp(previous.diveAmount, State.diveAmount, alpha);
            RenderState = rendered;
        }
    }
}
