using UnityEngine;

namespace Wings
{
    public sealed class BirdMotor : MonoBehaviour
    {
        public enum Activity { Flying, Approach, Perched, Takeoff }
        public FlightProfile profile;
        public PerchMotionProfile perchMotion;
        public FlightInput input;
        public Perch[] perches = new Perch[0];
        public Perch startingPerch;
        public Perch SelectedPerch { get; set; }
        public Perch LastSafePerch { get; private set; }
        public Activity CurrentActivity { get; private set; }
        public BirdActionState Action { get; private set; }
        public FlightContext Context { get; set; } = FlightContext.Shared;
        public FlightTuning Tuning => new FlightTuning(profile, Context);
        public FlightState RenderState { get; private set; }
        public FlightState State { get; private set; }
        public int Recoveries { get; private set; }
        public Vector3 origin = new Vector3(0, 18, -90);
        FlightState previous, assistStart;
        LandingTrajectory landingPath;
        Perch targetPerch;
        float assistTime, stuckTime, feedbackUntil, restingHeading;
        string feedback;
        float SettleSeconds => perchMotion != null ? perchMotion.settleSeconds : 0.32f;
        float CrouchSeconds => perchMotion != null ? perchMotion.crouchSeconds : 0.24f;
        float DepartureSeconds => perchMotion != null ? perchMotion.departureSeconds : 0.85f;
        float PushSpeed => perchMotion != null ? perchMotion.pushSpeed : 5;
        float PushUp => perchMotion != null ? perchMotion.pushUpSpeed : 3.2f;
        public string Status => Time.unscaledTime < feedbackUntil ? feedback : CurrentActivity == Activity.Perched ? "Rest, look around, then take flight." : "Point towards a perch to reveal Land here.";
        public bool HasAction => CurrentActivity != Activity.Flying || SelectedPerch != null;
        public string ActionLabel => Action.phase == BirdActionPhase.Touchdown ? "Settling…" : CurrentActivity == Activity.Approach ? "Cancel landing" : CurrentActivity == Activity.Perched ? "Take off" : CurrentActivity == Activity.Takeoff ? "Taking flight…" : "Land here";
        void Awake() { ResetFlight(); }
        void Start() { ResetToPerch(); }
        public void ResetToPerch()
        {
            feedbackUntil = 0; SelectedPerch = null;
            if (startingPerch != null && startingPerch.IsUsable) SetPerched(startingPerch, startingPerch.transform.eulerAngles.y);
            else Recover("Starting perch unavailable — moved to safety.");
        }
        // Explicit free-flight placement for validation/emergency recovery, never the normal game start.
        public void ResetFlight()
        {
            CurrentActivity = Activity.Flying; targetPerch = null; SelectedPerch = null; stuckTime = 0; feedbackUntil = 0;
            State = new FlightState { position = origin, velocity = Vector3.forward * (profile != null ? Tuning.cruiseSpeed : 14) };
            previous = RenderState = State; Action = new BirdActionState { phase = BirdActionPhase.Flight };
            transform.SetPositionAndRotation(State.position, State.Rotation);
        }
        void FixedUpdate()
        {
            if (input == null || input.Paused || profile == null) return;
            previous = State;
            if (CurrentActivity == Activity.Perched)
            {
                if (LastSafePerch == null || !LastSafePerch.IsUsable) Recover("Perch obstructed — moved to safety.");
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
            { Recover("Valley boundary — back to your safe perch."); return; }
            State = next; Action = new BirdActionState { phase = BirdActionPhase.Flight };
        }
        public void RequestPerchAction()
        {
            if (CurrentActivity == Activity.Approach) { CancelApproach(); return; }
            if (CurrentActivity == Activity.Takeoff) return;
            if (CurrentActivity == Activity.Perched)
            {
                if (LastSafePerch == null || !DepartureClear(LastSafePerch, State.heading)) { Notify("The departure path is blocked."); return; }
                targetPerch = LastSafePerch; assistStart = State; assistTime = 0; CurrentActivity = Activity.Takeoff;
                Action = RestAction(targetPerch, BirdActionPhase.Crouch); Notify("Crouch, push off, take flight."); return;
            }
            RequestLanding(SelectedPerch);
        }
        public bool RequestLanding(Perch perch)
        {
            if (CurrentActivity != Activity.Flying || !PerchTargeting.CanLand(perch, State)) return false;
            var path = new LandingTrajectory(State, perch.Point, perchMotion != null ? perchMotion.minimumApproach : 0.65f, perchMotion != null ? perchMotion.maximumApproach : 4.5f);
            if (!path.IsClear()) { Notify("That approach is obstructed. Try a clearer angle."); return false; }
            landingPath = path; targetPerch = perch; assistStart = State; assistTime = 0;
            CurrentActivity = Activity.Approach; Action = new BirdActionState { phase = BirdActionPhase.Approach, gazePoint = perch.FootPoint };
            Notify("Approaching " + perch.displayName + " — press again to cancel."); return true;
        }
        public void CancelApproach()
        {
            if (CurrentActivity != Activity.Approach || Action.phase == BirdActionPhase.Touchdown) return;
            CurrentActivity = Activity.Flying; targetPerch = null; SelectedPerch = null;
            var state = State; state.mode = FlightMode.Glide; State = state;
            Action = new BirdActionState { phase = BirdActionPhase.Flight }; Notify("Landing cancelled — free flight.");
        }
        public Vector3 DeparturePosition(float seconds, float heading, Vector3 start)
        {
            float t = Mathf.Clamp(seconds, 0, DepartureSeconds);
            var forward = Quaternion.Euler(0, heading, 0) * Vector3.forward;
            float acceleration = (Tuning.cruiseSpeed - PushSpeed) / DepartureSeconds;
            return start + forward * (PushSpeed*t + 0.5f*acceleration*t*t) + Vector3.up * (PushUp*t - 0.5f*t*t);
        }
        bool DepartureClear(Perch perch, float heading)
        {
            if (!perch.IsUsable) return false;
            var from = perch.Point;
            for (int i = 1; i <= 16; i++) { var to = DeparturePosition(DepartureSeconds*i/16f, heading, perch.Point); if (!FlightContact.PathClear(from,to)) return false; from = to; }
            return true;
        }
        public void AdvanceAssistance(float dt)
        {
            if (dt <= 0 || (CurrentActivity != Activity.Approach && CurrentActivity != Activity.Takeoff)) return;
            bool landing = CurrentActivity == Activity.Approach;
            if (targetPerch == null || !targetPerch.IsUsable)
            {
                if (landing && Action.phase != BirdActionPhase.Touchdown) { CancelApproach(); Notify("Perch obstructed — landing cancelled."); }
                else Recover("Contact obstructed — moved to safety."); return;
            }
            assistTime += dt;
            if (landing)
            {
                if (assistTime >= landingPath.duration)
                {
                    // Feet plant at contact; the articulated body settles without a vertical root descent.
                    if (!FlightContact.PathClear(State.position, targetPerch.Point)) { CancelApproach(); return; }
                    float settle = Mathf.Clamp01((assistTime - landingPath.duration) / Mathf.Max(0.1f, SettleSeconds));
                    var landed = State; landed.position = targetPerch.Point; landed.velocity = Vector3.zero; landed.pitch = 0; landed.bank = 0; landed.mode = FlightMode.Perched; State = landed;
                    Action = RestAction(targetPerch, BirdActionPhase.Touchdown); var action = Action;
                    action.progress = settle; action.flare = 1-settle; action.crouch = Mathf.Sin(settle*Mathf.PI)*0.5f; action.wingFold = settle; Action = action;
                    if (settle >= 1) SetPerched(targetPerch, State.heading);
                    return;
                }
                float t = Mathf.Clamp01(assistTime / landingPath.duration);
                var position = landingPath.Position(t);
                if (!FlightContact.PathClear(State.position, position)) { CancelApproach(); Notify("Approach blocked — free flight."); return; }
                var velocity = landingPath.Velocity(t);
                var next = State; next.position = position; next.velocity = velocity;
                Vector3 forward = Vector3.ProjectOnPlane(velocity, Vector3.up);
                if (forward.sqrMagnitude > 0.01f) next.heading = Mathf.Atan2(forward.x,forward.z)*Mathf.Rad2Deg;
                next.pitch = Mathf.Asin(Mathf.Clamp(velocity.normalized.y,-1,1))*Mathf.Rad2Deg;
                next.bank = Mathf.Lerp(assistStart.bank,0,Mathf.Clamp01(t*3)); next.yawRate = 0; next.mode = FlightMode.Approach; next.diveAmount = 0; next.flapEffort = 0.8f; State = next;
                float flare = Mathf.SmoothStep(0,1,Mathf.InverseLerp(perchMotion != null ? perchMotion.flareStart : 0.62f,1,t));
                Action = new BirdActionState { phase = flare > 0 ? BirdActionPhase.Flare : BirdActionPhase.Approach, progress=t, flare=flare, legReach=flare, gazePoint=targetPerch.FootPoint };
            }
            else
            {
                if (assistTime < CrouchSeconds)
                {
                    var action = RestAction(targetPerch,BirdActionPhase.Crouch); action.progress=assistTime/CrouchSeconds; action.crouch=action.progress; action.wingFold=1-action.progress; Action=action; return;
                }
                float seconds=assistTime-CrouchSeconds, t=Mathf.Clamp01(seconds/DepartureSeconds);
                var position=DeparturePosition(seconds,assistStart.heading,assistStart.position);
                if (!FlightContact.PathClear(State.position,position)) { Recover("Takeoff obstructed — back to safety."); return; }
                var forward=Quaternion.Euler(0,assistStart.heading,0)*Vector3.forward;
                var next=State; next.position=position; next.velocity=forward*Mathf.Lerp(PushSpeed,Tuning.cruiseSpeed,t)+Vector3.up*(PushUp-Mathf.Min(seconds,DepartureSeconds));
                next.pitch=Mathf.Asin(next.velocity.normalized.y)*Mathf.Rad2Deg; next.mode=FlightMode.Takeoff; next.flapEffort=1; State=next;
                Action=new BirdActionState { phase=t<0.22f ? BirdActionPhase.PushOff : BirdActionPhase.Departure, progress=t, legReach=1-Mathf.SmoothStep(0,1,t*2), gazePoint=position+forward*20 };
                if (t>=1) { CurrentActivity=Activity.Flying; targetPerch=null; SelectedPerch=null; Notify("Airborne — you're in control."); }
            }
        }
        BirdActionState RestAction(Perch perch, BirdActionPhase phase)
        {
            var side=Quaternion.Euler(0,State.heading,0)*Vector3.right*0.17f;
            return new BirdActionState { phase=phase, feetPlanted=true, leftFoot=perch.FootPoint-side, rightFoot=perch.FootPoint+side, legReach=1, grip=1, wingFold=1 };
        }
        void SetPerched(Perch perch,float heading)
        {
            LastSafePerch=perch; targetPerch=null; SelectedPerch=null; CurrentActivity=Activity.Perched; stuckTime=0; restingHeading=heading;
            State=new FlightState {position=perch.Point,heading=heading,mode=FlightMode.Perched}; previous=RenderState=State;
            Action=RestAction(perch,BirdActionPhase.Perched); Notify("Perched at "+perch.displayName+".");
        }
        public void Recover(string reason)
        {
            Recoveries++; stuckTime=0;
            if (LastSafePerch!=null && LastSafePerch.IsUsable) SetPerched(LastSafePerch,restingHeading);
            else if (startingPerch!=null && startingPerch.IsUsable) SetPerched(startingPerch,startingPerch.transform.eulerAngles.y);
            else
            {
                Vector3 safe=origin; int tries=0;
                while (!FlightContact.IsClear(safe) && tries++<60) safe+=Vector3.up*3;
                if (!FlightContact.IsClear(safe)) { if(input!=null) input.Paused=true; Notify("No clear recovery point. Pause and reset the scene."); return; }
                ResetFlight(); var state=State; state.position=safe; State=previous=state;
            }
            Notify(reason);
        }
        void Notify(string message) { feedback=message; feedbackUntil=Time.unscaledTime+4; }
        void Update()
        {
            float alpha=input!=null && input.Paused ? 1 : Mathf.Clamp01((Time.time-Time.fixedTime)/Time.fixedDeltaTime);
            transform.SetPositionAndRotation(Vector3.Lerp(previous.position,State.position,alpha),Quaternion.Slerp(previous.Rotation,State.Rotation,alpha));
            var rendered=State; rendered.bank=Mathf.Lerp(previous.bank,State.bank,alpha); rendered.flapEffort=Mathf.Lerp(previous.flapEffort,State.flapEffort,alpha); rendered.diveAmount=Mathf.Lerp(previous.diveAmount,State.diveAmount,alpha); RenderState=rendered;
        }
    }
}
