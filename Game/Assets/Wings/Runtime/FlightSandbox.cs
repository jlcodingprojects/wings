using UnityEngine;

namespace Wings
{
    [DefaultExecutionOrder(-50)]
    public sealed class FlightSandbox : MonoBehaviour
    {
        public FlightProfile assistedAsset, momentumAsset;
        public BirdMotor bird;
        public FlightInput input;
        public FlightCamera flightCamera;
        FlightProfile assisted, momentum;
        GUIStyle title, heading, body, small, button;
        int selected, tuningIndex;
        Vector2 scroll;
        public string ExperimentName => selected == 0 ? "A  /  Assisted" : "B  /  Momentum";
        public Rect Header => new Rect(20, 20, 345, 115);
        public Rect Buttons => new Rect(Screen.width - 460, 20, 440, 46);
        public Rect PerchButton => new Rect(Screen.width / 2f - 165, Screen.height - 124, 330, 40);
        Rect Panel => new Rect((Screen.width - 620) / 2f, Mathf.Max(30, (Screen.height - 700) / 2f), 620, Mathf.Min(700, Screen.height - 60));

        void Awake()
        {
            // Session copies: tuning never mutates authored assets.
            assisted = Instantiate(assistedAsset);
            momentum = Instantiate(momentumAsset);
            bird.profile = assisted;
            input.sandbox = this;
            input.Paused = true;
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            Time.fixedDeltaTime = 0.02f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        void Update() { input.Sample(); }
        void OnApplicationFocus(bool focus) { if (!focus) input.Paused = true; }
        void OnDestroy() { Destroy(assisted); Destroy(momentum); }

        public void TogglePause() { input.Paused = !input.Paused; }
        public void SwitchExperiment()
        {
            selected = 1 - selected;
            bird.profile = selected == 0 ? assisted : momentum;
        }
        public void ResetFlight() { bird.ResetFlight(); flightCamera.Recenter(true); }
        public bool PointerOverUI(Vector2 point) => input.Paused || Header.Contains(point) || Buttons.Contains(point) || PerchButton.Contains(point);
        public void ControllerTune(int row, int direction)
        {
            tuningIndex = (tuningIndex + row + 11) % 11;
            if (direction == 0) return;
            var p = bird.profile;
            switch (tuningIndex)
            {
                case 0: p.cruiseSpeed = Mathf.Clamp(p.cruiseSpeed + direction, 8, 24); break;
                case 1: p.turnRate = Mathf.Clamp(p.turnRate + direction * 5, 30, 110); break;
                case 2: p.response = Mathf.Clamp(p.response + direction * 0.5f, 0.5f, 8); break;
                case 3: flightCamera.distance = Mathf.Clamp(flightCamera.distance + direction, 5, 14); break;
                case 4: input.sensitivity = Mathf.Clamp(input.sensitivity + direction * 0.1f, 0.5f, 1.8f); break;
                case 5: p.diveBoost = Mathf.Clamp(p.diveBoost + direction, 0, 15); break;
                case 6: p.flapBoost = Mathf.Clamp(p.flapBoost + direction, 0, 12); break;
                case 7: input.invertY = !input.invertY; break;
                case 8: input.lookSensitivity = Mathf.Clamp(input.lookSensitivity + direction * 0.1f, 0.3f, 2); break;
                case 9: input.invertLookY = !input.invertLookY; break;
                case 10: flightCamera.recenterRate = Mathf.Clamp(flightCamera.recenterRate + direction * 0.2f, 0.3f, 3); break;
            }
        }

        void Styles()
        {
            if (title != null) return;
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            title = new GUIStyle(GUI.skin.label) { font = font, fontSize = 30, fontStyle = FontStyle.Bold };
            heading = new GUIStyle(GUI.skin.label) { font = font, fontSize = 20, fontStyle = FontStyle.Bold };
            body = new GUIStyle(GUI.skin.label) { font = font, fontSize = 16, wordWrap = true };
            small = new GUIStyle(body) { fontSize = 13 };
            button = new GUIStyle(GUI.skin.button) { font = font, fontSize = 16, fixedHeight = 38 };
            title.normal.textColor = heading.normal.textColor = new Color(0.94f, 0.93f, 0.84f);
            body.normal.textColor = small.normal.textColor = new Color(0.84f, 0.89f, 0.9f);
        }

        static void Fill(Rect rect, Color color)
        {
            Color saved = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = saved;
        }

        void OnGUI()
        {
            Styles();
            Fill(Header, new Color(0.035f, 0.09f, 0.12f, 0.88f));
            GUILayout.BeginArea(new Rect(36, 29, 315, 100));
            GUILayout.Label("WINGS  /  FLIGHT STUDY", heading);
            GUILayout.Label(ExperimentName + "   ·   " + (input.UsingGamepad ? "Controller" : "Mouse / keyboard"), body);
            GUILayout.Label($"{bird.State.mode}   ·   {bird.State.velocity.magnitude:0.0} m/s   ·   altitude {bird.State.position.y:0} m", small);
            GUILayout.Label("Early experiment — movement and art are provisional", small);
            GUILayout.EndArea();
            GUILayout.BeginArea(Buttons);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(input.Paused ? "Resume" : "Pause / tune", button)) TogglePause();
            if (GUILayout.Button("Reset bird", button)) ResetFlight();
            if (GUILayout.Button("Recenter", button)) flightCamera.Recenter();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            if (!input.Paused)
            {
                if (GUI.Button(PerchButton, bird.ActionLabel, button)) bird.RequestPerchAction();
                GUI.Label(new Rect(Screen.width / 2f - 350, Screen.height - 153, 700, 28), string.IsNullOrEmpty(input.DeviceNotice) ? bird.Status : input.DeviceNotice, body);
                Vector2 centre = new Vector2(Screen.width / 2f, Screen.height / 2f);
                Color guide = new Color(0.93f, 0.96f, 0.93f, 0.65f);
                Fill(new Rect(centre.x - 10, centre.y, 20, 1), guide);
                Fill(new Rect(centre.x, centre.y - 10, 1, 20), guide);
                GUILayout.BeginArea(new Rect(25, Screen.height - 72, Screen.width - 50, 55));
                GUILayout.Label("Find a marked roost. A / E or the Land button assists your approach.", body);
                GUILayout.Label("TAB / Y  compare     ESC / Start  pause     R / X  reset     C / right stick press  recenter     Water returns you to safety", small);
                GUILayout.EndArea();
                return;
            }
            Fill(new Rect(0, 0, Screen.width, Screen.height), new Color(0.015f, 0.035f, 0.05f, 0.60f));
            Fill(Panel, new Color(0.055f, 0.10f, 0.13f, 0.98f));
            GUILayout.BeginArea(new Rect(Panel.x + 30, Panel.y + 22, Panel.width - 60, Panel.height - 44));
            scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.Label("Find the feeling of flight", title);
            GUILayout.Label("Shared flight defaults. Bird and flock differences come later.", body);
            GUILayout.Space(12);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(selected == 0 ? "● A  Assisted" : "A  Assisted", button) && selected != 0) SwitchExperiment();
            if (GUILayout.Button(selected == 1 ? "● B  Momentum" : "B  Momentum", button) && selected != 1) SwitchExperiment();
            GUILayout.EndHorizontal();
            GUILayout.Label(selected == 0 ? "Quicker steering response and gentle levelling." : "Bank builds into a turn; velocity follows more gradually.", body);
            GUILayout.Space(12);
            var profile = bird.profile;
            profile.cruiseSpeed = Slider("Cruise speed", profile.cruiseSpeed, 8, 24, "m/s", 0);
            profile.turnRate = Slider("Turn rate", profile.turnRate, 30, 110, "deg/s", 1);
            profile.response = Slider("Response", profile.response, 0.5f, 8, "", 2);
            flightCamera.distance = Slider("Camera distance", flightCamera.distance, 5, 14, "m", 3);
            input.sensitivity = Slider("Steering sensitivity", input.sensitivity, 0.5f, 1.8f, "", 4);
            profile.diveBoost = Slider("Dive speed boost", profile.diveBoost, 0, 15, "m/s", 5);
            profile.flapBoost = Slider("Flap speed boost", profile.flapBoost, 0, 12, "m/s", 6);
            input.invertY = GUILayout.Toggle(input.invertY, (input.UsingGamepad && tuningIndex == 7 ? "> " : "") + "Invert climb / descent");
            input.lookSensitivity = Slider("Look sensitivity", input.lookSensitivity, 0.3f, 2, "", 8);
            input.invertLookY = GUILayout.Toggle(input.invertLookY, (input.UsingGamepad && tuningIndex == 9 ? "> " : "") + "Invert camera look");
            flightCamera.recenterRate = Slider("Camera recenter", flightCamera.recenterRate, 0.3f, 3, "", 10);
            GUILayout.Space(10);
            GUILayout.Label("Controller", heading);
            GUILayout.Label("Left stick: steer / climb    RT: flap    Right stick: look\nA: land / cancel / take off    B: cancel approach / back\nY: compare    X: reset    Start: pause    A: resume menu\nD-pad while paused selects / adjusts tuning. Press right stick to recenter.", body);
            GUILayout.Label("Mouse", heading);
            GUILayout.Label("Move around the centre guide to steer. Hold left to flap; release to glide. Hold right to look. Click Land / Take off or press E. C recentres. WASD + Space also work.", body);
            GUILayout.Space(8);
            if (GUILayout.Button("Fly  /  Resume", button)) TogglePause();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset tuning", button))
            {
                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(assistedAsset), assisted);
                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(momentumAsset), momentum);
                flightCamera.distance = 8; input.sensitivity = 1; input.invertY = false;
                input.lookSensitivity = 1; input.invertLookY = false; flightCamera.recenterRate = 1.2f;
            }
            if (GUILayout.Button("Quit", button)) Application.Quit();
            GUILayout.EndHorizontal();
            GUILayout.Label("What feels right: response, weight, speed, climb and camera? Changes here last for this session.", small);
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        float Slider(string label, float value, float min, float max, string unit, int index)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label((input.UsingGamepad && tuningIndex == index ? "> " : "") + label, body, GUILayout.Width(170));
            value = GUILayout.HorizontalSlider(value, min, max, GUILayout.Width(220));
            GUILayout.Label($"{value:0.0} {unit}", small, GUILayout.Width(100));
            GUILayout.EndHorizontal();
            return value;
        }
    }
}
