using UnityEngine;
using UnityEngine.InputSystem;

namespace Wings
{
    public sealed class FlightInput : MonoBehaviour
    {
        public FlightIntent Intent { get; private set; }
        public Vector2 Look { get; private set; }
        public bool UsingGamepad { get; private set; }
        public bool Paused { get; set; } = true;
        public FlightSandbox sandbox;
        public float sensitivity = 1;
        public bool invertY;
        public float lookSensitivity = 1;
        public bool invertLookY;
        public bool LookHeld { get; private set; }
        float noticeUntil;
        public string DeviceNotice => Time.unscaledTime < noticeUntil ? "Controller disconnected — mouse steering is available." : "";

        public static FlightIntent Map(Vector2 steer, float flap, bool consumed, bool invert)
        {
            if (consumed) return default;
            return new FlightIntent { steer = Vector2.ClampMagnitude(new Vector2(steer.x, invert ? -steer.y : steer.y), 1), flap = Mathf.Clamp01(flap) };
        }

        public void Sample()
        {
            bool wasPaused = Paused;
            Gamepad pad = Gamepad.current;
            Mouse mouse = Mouse.current;
            Keyboard keyboard = Keyboard.current;
            if (pad != null && (pad.leftStick.ReadValue().sqrMagnitude > 0.02f || pad.rightStick.ReadValue().sqrMagnitude > 0.02f || pad.rightTrigger.ReadValue() > 0.05f || pad.buttonSouth.wasPressedThisFrame)) UsingGamepad = true;
            if (mouse != null && (mouse.delta.ReadValue().sqrMagnitude > 1 || mouse.leftButton.wasPressedThisFrame)) UsingGamepad = false;
            if (pad == null) { if (UsingGamepad) noticeUntil = Time.unscaledTime + 6; UsingGamepad = false; }

            if ((keyboard?.escapeKey.wasPressedThisFrame ?? false) || (pad?.startButton.wasPressedThisFrame ?? false)) sandbox.TogglePause();
            if ((keyboard?.tabKey.wasPressedThisFrame ?? false) || (pad?.buttonNorth.wasPressedThisFrame ?? false)) sandbox.SwitchExperiment();
            if ((keyboard?.rKey.wasPressedThisFrame ?? false) || (pad?.buttonWest.wasPressedThisFrame ?? false)) sandbox.ResetFlight();
            if (Paused && ((pad?.buttonSouth.wasPressedThisFrame ?? false) || (pad?.buttonEast.wasPressedThisFrame ?? false))) sandbox.TogglePause();
            if (!wasPaused && !Paused && ((pad?.buttonSouth.wasPressedThisFrame ?? false) || (keyboard?.eKey.wasPressedThisFrame ?? false))) sandbox.bird.RequestPerchAction();
            if (!Paused && (pad?.buttonEast.wasPressedThisFrame ?? false)) sandbox.bird.CancelApproach();
            if ((keyboard?.cKey.wasPressedThisFrame ?? false) || (pad?.rightStickButton.wasPressedThisFrame ?? false)) sandbox.flightCamera.Recenter();
            if (Paused)
            {
                if (pad != null)
                {
                    int row = (pad.dpad.down.wasPressedThisFrame ? 1 : 0) - (pad.dpad.up.wasPressedThisFrame ? 1 : 0);
                    int direction = (pad.dpad.right.wasPressedThisFrame ? 1 : 0) - (pad.dpad.left.wasPressedThisFrame ? 1 : 0);
                    if (row != 0 || direction != 0) { UsingGamepad = true; sandbox.ControllerTune(row, direction); }
                }
                Intent = default; Look = default; LookHeld = false; return;
            }

            Vector2 steer = Vector2.zero;
            float flap = 0;
            Look = Vector2.zero;
            LookHeld = false;
            bool consumed = false;
            if (UsingGamepad && pad != null)
            {
                steer = pad.leftStick.ReadValue();
                flap = pad.rightTrigger.ReadValue();
                Look = pad.rightStick.ReadValue() * 95 * Time.unscaledDeltaTime;
                LookHeld = pad.rightStick.ReadValue().sqrMagnitude > 0.01f;
            }
            else if (mouse != null)
            {
                Vector2 point = mouse.position.ReadValue();
                Vector2 guiPoint = new Vector2(point.x, Screen.height - point.y);
                consumed = sandbox.PointerOverUI(guiPoint);
                bool looking = mouse.rightButton.isPressed;
                LookHeld = looking && !consumed;
                if (looking && !consumed) Look = mouse.delta.ReadValue() * 0.12f;
                Vector2 centre = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                steer = looking ? Vector2.zero : (point - centre) / (Mathf.Min(Screen.width, Screen.height) * 0.28f);
                // Radial dead zone makes it easy to settle into neutral forward flight.
                steer = steer.magnitude < 0.07f ? Vector2.zero : Vector2.ClampMagnitude(steer, 1);
                flap = mouse.leftButton.isPressed ? 1 : 0;
            }
            if (!UsingGamepad && keyboard != null)
            {
                Vector2 keys = new Vector2((keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0), (keyboard.wKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed ? 1 : 0));
                if (keys.sqrMagnitude > 0) { steer = keys; consumed = false; }
                if (keyboard.spaceKey.isPressed && !consumed) flap = 1;
            }
            Intent = Map(steer * sensitivity, flap, consumed, invertY);
            Look = new Vector2(Look.x, invertLookY ? -Look.y : Look.y) * lookSensitivity;
        }
    }
}
