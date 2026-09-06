using UnityEngine;

namespace Wings
{
    [DefaultExecutionOrder(10)]
    public sealed class BirdPresentation : MonoBehaviour
    {
        public BirdMotor motor;
        public Transform visual;
        public BirdRig rig;
        public BirdActionState Pose { get; private set; }
        float phase, fold = 1, bodyPitch = 28;
        Quaternion headRest;
        BirdActionPhase previousPhase;
        void Awake() { if (rig != null && rig.head != null) headRest = rig.head.localRotation; }
        void Update()
        {
            if (rig == null || !rig.IsComplete) return;
            var state = motor.RenderState;
            var action = motor.Action;
            bool rest = action.phase == BirdActionPhase.Perched;
            bool paused = motor.input != null && motor.input.Paused;
            float dt = paused ? 0 : Time.deltaTime;
            float blend = 1 - Mathf.Exp(-dt * 14);
            float effort = action.phase == BirdActionPhase.Flight ? state.flapEffort : action.feetPlanted ? 0 : 1;
            if (action.phase == BirdActionPhase.PushOff && previousPhase != BirdActionPhase.PushOff) phase = Mathf.PI*0.5f;
            else phase += dt * Mathf.Lerp(2, 6.5f, effort) * Mathf.PI * 2;
            previousPhase = action.phase;
            fold = Mathf.Lerp(fold, action.wingFold, blend);
            float pitch = action.feetPlanted ? Mathf.Lerp(28, 52, action.flare) - action.crouch * 20 : Mathf.Lerp(state.pitch, 58, action.flare);
            bodyPitch = Mathf.Lerp(bodyPitch, pitch, blend);
            Vector3 gaze = action.gazePoint;
            if (gaze == Vector3.zero)
            {
                Vector2 intent = motor.input != null ? motor.input.Intent.steer : Vector2.zero;
                var direction = state.Rotation * Quaternion.Euler(-intent.y * 25, intent.x * 40, 0) * Vector3.forward;
                if (motor.SelectedPerch != null) gaze = motor.SelectedPerch.FootPoint;
                else if (rest && Camera.main != null) gaze = motor.State.position + Camera.main.transform.forward * 20;
                else gaze = motor.State.position + direction * 20;
            }
            action.gazePoint = gaze; Pose = action;
            if (rig.useAuthoredAnimation && rig.authoredAnimator != null)
            {
                // Generic clips own the bones in this mode. Root motion remains disabled.
                rig.authoredAnimator.applyRootMotion = false;
                foreach (var parameter in rig.authoredAnimator.parameters)
                {
                    if (parameter.name == "ActionPhase" && parameter.type == AnimatorControllerParameterType.Int) rig.authoredAnimator.SetInteger(parameter.nameHash, (int)action.phase);
                    if (parameter.name == "ActionProgress" && parameter.type == AnimatorControllerParameterType.Float) rig.authoredAnimator.SetFloat(parameter.nameHash, action.progress);
                    if (parameter.name == "FlapEffort" && parameter.type == AnimatorControllerParameterType.Float) rig.authoredAnimator.SetFloat(parameter.nameHash, effort);
                    if (parameter.name == "Flare" && parameter.type == AnimatorControllerParameterType.Float) rig.authoredAnimator.SetFloat(parameter.nameHash, action.flare);
                    if (parameter.name == "Speed" && parameter.type == AnimatorControllerParameterType.Float) rig.authoredAnimator.SetFloat(parameter.nameHash, state.velocity.magnitude);
                }
                return;
            }
            // Root carries path orientation; torso pitch is independently free to brake nose-up.
            visual.localRotation = Quaternion.Euler(state.pitch, 0, state.bank);
            rig.body.localPosition = new Vector3(0, -action.crouch * 0.14f, 0);
            rig.body.localRotation = Quaternion.Euler(-bodyPitch, 0, 0);
            float sweep = Mathf.Sin(phase) * Mathf.Lerp(6, 52, effort) * (1-fold);
            if (action.phase == BirdActionPhase.Crouch) sweep = 65 * action.progress;
            Wing(rig.leftWing, -1, sweep, fold, action.flare);
            Wing(rig.rightWing, 1, sweep, fold, action.flare);
            rig.tail.localRotation = Quaternion.Euler(Mathf.Lerp(8, -25, action.flare), 0, 0);
            for (int i=0; i<rig.tailFeathers.Length; i++) rig.tailFeathers[i].localRotation = Quaternion.Euler(0,(i-(rig.tailFeathers.Length-1)*0.5f)*Mathf.Lerp(4,14,action.flare),0);
            Leg(rig.leftLeg,-1,action); Leg(rig.rightLeg,1,action);
            Vector3 localGaze = rig.head.parent.InverseTransformDirection(gaze-rig.head.position).normalized;
            float yaw=Mathf.Clamp(Mathf.Atan2(localGaze.x,localGaze.z)*Mathf.Rad2Deg,-65,65);
            float headPitch=Mathf.Clamp(-Mathf.Asin(localGaze.y)*Mathf.Rad2Deg,-55,55);
            rig.head.localRotation=Quaternion.Slerp(rig.head.localRotation,headRest*Quaternion.Euler(headPitch,yaw,0),paused ? 1 : blend);
        }
        static void Wing(BirdRig.Wing wing,int side,float sweep,float fold,float flare)
        {
            wing.shoulder.localRotation=Quaternion.Euler(flare*-18,side*fold*68,side*sweep);
            wing.elbow.localRotation=Quaternion.Euler(0,-side*fold*135,0);
            wing.wrist.localRotation=Quaternion.Euler(0,side*fold*115,side*Mathf.Max(0,sweep)*0.25f);
        }
        void Leg(BirdRig.Leg leg,int side,BirdActionState action)
        {
            Vector3 hip = new Vector3(side*0.17f,-0.12f,-0.08f);
            Vector3 target = action.feetPlanted ? visual.InverseTransformPoint(side<0 ? action.leftFoot : action.rightFoot)
                : Vector3.Lerp(new Vector3(side*0.17f,-0.18f,-0.48f),new Vector3(side*0.17f,-0.58f,0.35f),action.legReach);
            // A simple two-link solve keeps the toes fixed during body compression.
            hip += rig.body.localPosition;
            Vector3 delta=target-hip; float distance=Mathf.Clamp(delta.magnitude,0.05f,0.69f);
            Vector3 direction=delta.normalized;
            Vector3 bend=Vector3.ProjectOnPlane(Vector3.back,direction).normalized;
            Vector3 knee=hip+direction*distance*0.5f+bend*Mathf.Sqrt(Mathf.Max(0,0.35f*0.35f-distance*distance*0.25f));
            PlaceBone(leg.hip,visual.TransformPoint(hip),visual.TransformPoint(knee));
            PlaceBone(leg.knee,visual.TransformPoint(knee),visual.TransformPoint(target));
            leg.ankle.position=visual.TransformPoint(target);
            leg.ankle.rotation=Quaternion.Euler(0,motor.State.heading,0);
            leg.foot.localRotation=Quaternion.identity;
            foreach(var toe in leg.toes) toe.localRotation=Quaternion.Euler(action.grip*28,0,0);
        }
        static void PlaceBone(Transform bone,Vector3 from,Vector3 to)
        {
            bone.position=from;
            bone.rotation=Quaternion.FromToRotation(Vector3.down,(to-from).normalized);
            // Bone meshes are authored 0.35 m long along -Y. Keep the hierarchy unscaled.
            var mesh=bone.GetChild(0); mesh.localScale=new Vector3(0.065f,Vector3.Distance(from,to),0.065f);
            mesh.localPosition=Vector3.down*Vector3.Distance(from,to)*0.5f;
        }
    }
}
