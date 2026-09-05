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
            string[] args = Environment.GetCommandLineArgs();
            int argument = Array.IndexOf(args, "-wingsSmoke");
            if (argument < 0) { enabled = false; yield break; }
            folder = argument + 1 < args.Length ? args[argument + 1] : Application.persistentDataPath;
            Directory.CreateDirectory(folder);
            report = new Report { unity = Application.unityVersion, graphics = SystemInfo.graphicsDeviceType.ToString(), device = SystemInfo.graphicsDeviceName, physicalGamepads = Gamepad.all.Count };
            sandbox = FindFirstObjectByType<FlightSandbox>();
            yield return null;
            ScreenCapture.CaptureScreenshot(Path.Combine(folder, "checkpoint1c-menu.png"));
            yield return new WaitForSecondsRealtime(1);
            if (!Check(sandbox != null, "Scene bootstrap present")) yield break;
            if (!Check(SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null, "Graphics-enabled player")) yield break;
            sandbox.input.Paused = false;
            pad = InputSystem.AddDevice<Gamepad>();
            Vector3 start = sandbox.bird.State.position;
            for (int i = 0; i < 90; i++)
            {
                InputSystem.QueueStateEvent(pad, new GamepadState { leftStick = new Vector2(0.3f, 0.12f), rightTrigger = 0.7f });
                yield return null;
            }
            if (!Check(sandbox.input.UsingGamepad && sandbox.input.Intent.flap > 0.5f && Vector3.Distance(start, sandbox.bird.State.position) > 4, "Synthetic controller reaches input and moves bird")) yield break;
            InputSystem.QueueStateEvent(pad, new GamepadState());
            sandbox.SwitchExperiment();
            if (!Check(sandbox.bird.profile.experiment == FlightExperiment.Momentum, "Mode B is selectable at runtime")) yield break;
            for (int i = 0; i < 40; i++) yield return null;
            var originalOrigin = sandbox.bird.origin;
            sandbox.bird.origin = new Vector3(0, 100, -90);
            sandbox.ResetFlight();
            for (int i = 0; i < 110; i++)
            {
                InputSystem.QueueStateEvent(pad, new GamepadState { leftStick = Vector2.down });
                yield return null;
            }
            if (!Check(sandbox.bird.State.mode == FlightMode.Dive && sandbox.bird.State.velocity.magnitude > sandbox.bird.Tuning.cruiseSpeed + 1 && sandbox.bird.State.velocity.magnitude <= sandbox.bird.Tuning.MaximumSpeed + 0.01f, "Dive increases speed within the configured limit")) yield break;
            for (int i = 0; i < 160; i++)
            {
                InputSystem.QueueStateEvent(pad, new GamepadState { rightTrigger = 1 });
                yield return null;
            }
            if (!Check(sandbox.bird.State.mode == FlightMode.Flap && Mathf.Abs(sandbox.bird.State.pitch) < 2, "Flapping remains available and release of steering levels the bird")) yield break;
            sandbox.bird.origin = originalOrigin;
            sandbox.ResetFlight();
            InputSystem.QueueStateEvent(pad, new GamepadState());
            yield return null;
            sandbox.input.Paused = true;
            yield return PressAction();
            if (!Check(!sandbox.input.Paused && sandbox.bird.CurrentActivity == BirdMotor.Activity.Flying, "Controller resume does not also request landing")) yield break;
            yield return PressAction();
            if (!Check(sandbox.bird.CurrentActivity == BirdMotor.Activity.Approach, "Controller requests nearby landing")) yield break;
            yield return PressAction();
            if (!Check(sandbox.bird.CurrentActivity == BirdMotor.Activity.Flying, "Second action cancels approach")) yield break;
            yield return PressAction();
            yield return new WaitForSeconds(2.6f);
            if (!Check(sandbox.bird.CurrentActivity == BirdMotor.Activity.Perched && sandbox.bird.LastSafePerch != null, "Assisted landing reaches a safe perch")) yield break;
            ScreenCapture.CaptureScreenshot(Path.Combine(folder, "checkpoint1c-perched.png"));
            yield return new WaitForSecondsRealtime(1);
            yield return PressAction();
            yield return new WaitForSeconds(1.5f);
            if (!Check(sandbox.bird.CurrentActivity == BirdMotor.Activity.Flying, "Controller takeoff returns to free flight")) yield break;
            sandbox.bird.origin = new Vector3(115, 0.9f, 40);
            sandbox.ResetFlight();
            int recoveries = sandbox.bird.Recoveries;
            for (int i = 0; i < 120 && sandbox.bird.Recoveries == recoveries; i++)
            {
                InputSystem.QueueStateEvent(pad, new GamepadState { leftStick = Vector2.down });
                yield return null;
            }
            if (!Check(sandbox.bird.Recoveries > recoveries && sandbox.bird.CurrentActivity == BirdMotor.Activity.Perched && sandbox.bird.Status.StartsWith("Water contact"), "Water contact returns bird to last safe perch")) yield break;
            sandbox.bird.origin = originalOrigin;
            sandbox.ResetFlight();
            InputSystem.RemoveDevice(pad); pad = null;
            mouse = InputSystem.AddDevice<Mouse>();
            var mouseState = new MouseState { position = new Vector2(Screen.width * 0.62f, Screen.height * 0.55f) }.WithButton(MouseButton.Left);
            InputSystem.QueueStateEvent(mouse, mouseState);
            yield return null; yield return null;
            if (!Check(!sandbox.input.UsingGamepad && sandbox.input.Intent.flap > 0.9f && sandbox.input.Intent.steer.x > 0, "Mouse fallback steers and flaps after controller removal")) yield break;
            InputSystem.QueueStateEvent(mouse, new MouseState { position = new Vector2(40, Screen.height - 40) }.WithButton(MouseButton.Left));
            yield return null; yield return null;
            if (!Check(sandbox.input.Intent.flap == 0 && sandbox.input.Intent.steer == Vector2.zero, "Pointer over HUD does not also fly/flap")) yield break;
            InputSystem.QueueStateEvent(mouse, new MouseState { position = new Vector2(Screen.width / 2f, 104) }.WithButton(MouseButton.Left));
            yield return null; yield return null;
            if (!Check(sandbox.input.Intent.flap == 0 && sandbox.input.Intent.steer == Vector2.zero, "Pointer over landing action does not also fly/flap")) yield break;
            InputSystem.QueueStateEvent(mouse, new MouseState { position = new Vector2(Screen.width * 0.7f, Screen.height / 2f) }.WithButton(MouseButton.Right));
            yield return null; yield return null;
            if (!Check(sandbox.input.LookHeld && sandbox.input.Intent.steer == Vector2.zero, "Mouse orbit suspends pointer steering")) yield break;
            sandbox.input.Paused = true;
            var pausedPosition = sandbox.bird.State.position;
            yield return new WaitForSecondsRealtime(0.2f);
            if (!Check(Vector3.Distance(pausedPosition, sandbox.bird.State.position) < 0.001f, "Pause freezes the bird")) yield break;
            sandbox.input.Paused = false;
            sandbox.ResetFlight();
            InputSystem.QueueStateEvent(mouse, new MouseState { position = new Vector2(Screen.width / 2f, Screen.height / 2f) });
            for (int i = 0; i < 90; i++) yield return null;
            ScreenCapture.CaptureScreenshot(Path.Combine(folder, "checkpoint1c-flight.png"));
            yield return new WaitForSecondsRealtime(1);
            if (!Check(File.Exists(Path.Combine(folder, "checkpoint1c-flight.png")), "Rendered flight capture written")) yield break;
            if (!Check(HasVisibleImage(Path.Combine(folder, "checkpoint1c-flight.png")) && HasVisibleImage(Path.Combine(folder, "checkpoint1c-menu.png")), "Captures contain visible rendered content")) yield break;
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
