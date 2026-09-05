using UnityEngine;

namespace Wings
{
    [DefaultExecutionOrder(10)]
    public sealed class BirdPresentation : MonoBehaviour
    {
        public BirdMotor motor;
        public Transform visual, leftWing, rightWing;
        float phase, amplitude, tuck;
        void Update()
        {
            var state = motor.RenderState;
            if (!motor.input.Paused)
            {
                phase += Time.deltaTime * Mathf.Lerp(2, 13, state.flapEffort);
                float blend = 1 - Mathf.Exp(-Time.deltaTime * 7);
                amplitude = Mathf.Lerp(amplitude, state.mode == FlightMode.Perched ? 0 : Mathf.Lerp(4, 27, state.flapEffort), blend);
                tuck = Mathf.Lerp(tuck, state.mode == FlightMode.Perched ? 65 : state.diveAmount * (1 - state.flapEffort) * 18, blend);
            }
            visual.localRotation = Quaternion.Euler(0, 0, state.bank);
            float sweep = Mathf.Sin(phase) * amplitude;
            leftWing.localRotation = Quaternion.Euler(0, -tuck, -sweep);
            rightWing.localRotation = Quaternion.Euler(0, tuck, sweep);
        }
    }
}
