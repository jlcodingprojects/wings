using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Wings
{
    // Opt-in player integration check. Exercises real Input System devices, never runs in ordinary play.
    public sealed class RuntimeSmoke : MonoBehaviour
    {
        [Serializable] class Report
        {
            public string unity, graphics, device, result;
            public int physicalGamepads;
            public List<string> checks = new List<string>();
        }
        string folder;
        Report report;
        FlightSandbox sandbox;
        Gamepad pad;
        Mouse mouse;

        IEnumerator Start()
        {
            string[] args=Environment.GetCommandLineArgs(); int argument=Array.IndexOf(args,"-wingsSmoke");
            if(argument<0) { enabled=false; yield break; }
            folder=argument+1<args.Length?args[argument+1]:Application.persistentDataPath;
            Directory.CreateDirectory(folder);
            report=new Report {unity=Application.unityVersion,graphics=SystemInfo.graphicsDeviceType.ToString(),device=SystemInfo.graphicsDeviceName,physicalGamepads=Gamepad.all.Count};
            sandbox=FindFirstObjectByType<FlightSandbox>();
            yield return null; yield return null;
            if(!Check(sandbox!=null,"Scene bootstrap present")) yield break;
            var bird=sandbox.bird; var rig=bird.GetComponent<BirdPresentation>().rig;
            if(!Check(bird.CurrentActivity==BirdMotor.Activity.Perched && bird.State.velocity==Vector3.zero,"Game starts perched with zero velocity")) yield break;
            if(!Check(rig.IsComplete,"Articulated wing, head, tail, leg and toe bindings present")) yield break;
            if(!Check(Vector3.Distance(rig.leftLeg.ankle.position,bird.Action.leftFoot)<0.04f,"Resting foot is planted on the branch")) yield break;
            ScreenCapture.CaptureScreenshot(Path.Combine(folder,"checkpoint1d-start.png"));
            yield return new WaitForSecondsRealtime(0.5f);
            sandbox.input.Paused=true;
            ScreenCapture.CaptureScreenshot(Path.Combine(folder,"checkpoint1d-menu.png"));
            pad=InputSystem.AddDevice<Gamepad>();
            yield return PressAction();
            if(!Check(!sandbox.input.Paused && bird.CurrentActivity==BirdMotor.Activity.Perched,"Controller resume does not also take off")) yield break;
            yield return PressAction();
            if(!Check(bird.CurrentActivity==BirdMotor.Activity.Takeoff && bird.Action.feetPlanted,"Takeoff begins with planted feet and a crouch")) yield break;
            ScreenCapture.CaptureScreenshot(Path.Combine(folder,"checkpoint1d-crouch.png"));
            bool pushed=false,departed=false;
            for(int i=0;i<180 && bird.CurrentActivity!=BirdMotor.Activity.Flying;i++)
            {
                sandbox.flightCamera.SetOrbit(65,0);
                if(!pushed && bird.Action.phase==BirdActionPhase.PushOff) { pushed=true; ScreenCapture.CaptureScreenshot(Path.Combine(folder,"checkpoint1d-push.png")); }
                if(!departed && bird.Action.phase==BirdActionPhase.Departure && bird.Action.progress>0.5f) { departed=true; ScreenCapture.CaptureScreenshot(Path.Combine(folder,"checkpoint1d-departure.png")); }
                yield return null;
            }
            if(!Check(pushed && departed && bird.State.velocity.magnitude>8,"Leg push transitions into powered forward departure")) yield break;
            var perch=bird.perches[1];
            bird.origin=perch.Point+new Vector3(0,2,-25); bird.ResetFlight(); sandbox.flightCamera.Recenter(true);
            InputSystem.QueueStateEvent(pad,new GamepadState { rightTrigger=0.1f }); yield return null; yield return null;
            InputSystem.QueueStateEvent(pad,new GamepadState()); yield return null;
            if(!Check(bird.SelectedPerch==perch,"Controller flight aim selects the perch ahead")) yield break;
            yield return PressAction();
            if(!Check(bird.CurrentActivity==BirdMotor.Activity.Approach && bird.State.velocity.magnitude>8,"Landing acquisition keeps forward motion")) yield break;
            yield return PressAction();
            if(!Check(bird.CurrentActivity==BirdMotor.Activity.Flying,"Second action cancels landing before contact")) yield break;
            // Reposition for a repeatable complete approach; normal play selects this through aim.
            bird.ResetFlight(); sandbox.flightCamera.Recenter(true); yield return null; yield return null;
            yield return PressAction();
            bool flare=false,touchdown=false;
            for(int i=0;i<300 && bird.CurrentActivity!=BirdMotor.Activity.Perched;i++)
            {
                sandbox.flightCamera.SetOrbit(70,0);
                if(i==15) ScreenCapture.CaptureScreenshot(Path.Combine(folder,"checkpoint1d-approach.png"));
                if(!flare && bird.Action.flare>0.65f) { flare=true; ScreenCapture.CaptureScreenshot(Path.Combine(folder,"checkpoint1d-flare.png")); }
                if(!touchdown && bird.Action.phase==BirdActionPhase.Touchdown) { touchdown=true; ScreenCapture.CaptureScreenshot(Path.Combine(folder,"checkpoint1d-contact.png")); }
                yield return null;
            }
            if(!Check(flare && touchdown && bird.CurrentActivity==BirdMotor.Activity.Perched,"Forward approach flares, plants feet, then settles")) yield break;
            yield return new WaitForSeconds(0.3f);
            if(!Check(Vector3.Distance(rig.leftLeg.ankle.position,bird.Action.leftFoot)<0.04f,"Foot contact remains fixed after settling")) yield break;
            ScreenCapture.CaptureScreenshot(Path.Combine(folder,"checkpoint1d-perched.png"));
            yield return new WaitForSecondsRealtime(0.5f);
            InputSystem.RemoveDevice(pad); pad=null;
            mouse=InputSystem.AddDevice<Mouse>();
            bird.ResetFlight(); sandbox.flightCamera.Recenter(true);
            InputSystem.QueueStateEvent(mouse,new MouseState {position=new Vector2(Screen.width/2f,Screen.height/2f)});
            yield return null; yield return null;
            var camera=sandbox.flightCamera.GetComponent<Camera>();
            var screen=camera.WorldToScreenPoint(perch.Point);
            InputSystem.QueueStateEvent(mouse,new MouseState {position=new Vector2(screen.x,screen.y)});
            yield return null; yield return null;
            if(!Check(bird.SelectedPerch==perch,"Mouse pointing at a perch reveals Land here")) yield break;
            var actionButton=sandbox.PerchButton;
            InputSystem.QueueStateEvent(mouse,new MouseState {position=new Vector2(actionButton.center.x,Screen.height-actionButton.center.y)}.WithButton(MouseButton.Left));
            yield return null;
            if(!Check(sandbox.input.Intent.flap==0 && sandbox.input.Intent.steer==Vector2.zero && bird.CurrentActivity==BirdMotor.Activity.Approach,"Land here click starts landing without steering or flapping")) yield break;
            InputSystem.QueueStateEvent(mouse,new MouseState {position=new Vector2(actionButton.center.x,Screen.height-actionButton.center.y)});
            yield return null; yield return null;
            if(!Check(bird.CurrentActivity==BirdMotor.Activity.Approach,"Releasing Land here does not cancel the approach")) yield break;
            bird.CancelApproach();
            InputSystem.QueueStateEvent(mouse,new MouseState {position=new Vector2(Screen.width*0.7f,Screen.height/2f)}.WithButton(MouseButton.Right));
            yield return null; yield return null;
            if(!Check(sandbox.input.LookHeld && sandbox.input.Intent.steer==Vector2.zero,"Mouse orbit suspends pointer steering")) yield break;
            sandbox.SwitchExperiment();
            if(!Check(bird.profile.experiment==FlightExperiment.Momentum,"Both flight experiments remain available")) yield break;
            sandbox.ResetFlight();
            if(!Check(bird.CurrentActivity==BirdMotor.Activity.Perched,"Reset returns to the opening perch")) yield break;
            sandbox.input.Paused=true; var position=bird.State.position; yield return new WaitForSecondsRealtime(0.2f);
            if(!Check(bird.State.position==position,"Pause preserves contact")) yield break;
            foreach(string capture in new[]{"start","crouch","push","departure","approach","flare","contact","perched"})
                if(!Check(HasVisibleImage(Path.Combine(folder,"checkpoint1d-"+capture+".png")),"Rendered "+capture+" capture")) yield break;
            Finish(true);
        }
        IEnumerator PressAction()
        {
            InputSystem.QueueStateEvent(pad, new GamepadState().WithButton(GamepadButton.South));
            yield return null; yield return null;
            InputSystem.QueueStateEvent(pad, new GamepadState());
            yield return null; yield return null;
        }

        bool Check(bool value, string label)
        {
            report.checks.Add((value ? "PASS: " : "FAIL: ") + label);
            if (!value) Finish(false);
            return value;
        }
        static bool HasVisibleImage(string path)
        {
            if (!File.Exists(path)) return false;
            var image = new Texture2D(2, 2);
            try
            {
                if (!image.LoadImage(File.ReadAllBytes(path))) return false;
                var pixels = image.GetPixels32();
                int min = 765, max = 0;
                for (int i = 0; i < pixels.Length; i += 73)
                {
                    int value = pixels[i].r + pixels[i].g + pixels[i].b;
                    min = Math.Min(min, value); max = Math.Max(max, value);
                }
                return max > 100 && max - min > 75;
            }
            finally { Destroy(image); }
        }
        void Finish(bool passed)
        {
            if (pad != null && pad.added) InputSystem.RemoveDevice(pad);
            if (mouse != null && mouse.added) InputSystem.RemoveDevice(mouse);
            report.result = passed ? "passed" : "failed";
            File.WriteAllText(Path.Combine(folder, "player-smoke.json"), JsonUtility.ToJson(report, true));
            Debug.Log("WINGS_PLAYER_SMOKE_" + (passed ? "PASS" : "FAIL"));
            Application.Quit(passed ? 0 : 1);
        }
    }
}
