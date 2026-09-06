using NUnit.Framework;
using UnityEngine;

namespace Wings.Tests
{
    public sealed class FlightTests
    {
        FlightProfile profile;
        [SetUp] public void Setup() { profile = ScriptableObject.CreateInstance<FlightProfile>(); }
        [TearDown] public void Cleanup() { Object.DestroyImmediate(profile); }
        FlightState State => new FlightState { position = new Vector3(0, 20, 0), velocity = Vector3.forward * profile.cruiseSpeed };

        [Test] public void ConsumedPointerCannotSteerOrFlap()
        {
            var intent = FlightInput.Map(Vector2.one, 1, true, false);
            Assert.That(intent.steer, Is.EqualTo(Vector2.zero));
            Assert.That(intent.flap, Is.Zero);
        }
        [Test] public void InversionOnlyChangesVerticalIntentAndDiagonalIsBounded()
        {
            var normal = FlightInput.Map(new Vector2(1, 1), 1, false, false);
            var inverted = FlightInput.Map(new Vector2(1, 1), 1, false, true);
            Assert.That(inverted.steer.x, Is.EqualTo(normal.steer.x));
            Assert.That(inverted.steer.y, Is.EqualTo(-normal.steer.y));
            Assert.That(inverted.steer.magnitude, Is.EqualTo(1).Within(0.0001));
        }
        [TestCase(FlightExperiment.Assisted)] [TestCase(FlightExperiment.Momentum)]
        public void RightAndClimbIntentProducesRightTurnAndAscent(FlightExperiment mode)
        {
            profile.experiment = mode;
            var state = State;
            for (int i = 0; i < 100; i++) state = FlightSimulation.Step(state, new FlightIntent { steer = new Vector2(0.5f, 0.5f), flap = 1 }, profile, 0.02f);
            Assert.That(state.position.x, Is.GreaterThan(1));
            Assert.That(state.position.y, Is.GreaterThan(22));
            Assert.That(state.bank, Is.LessThan(0));
        }
        [TestCase(FlightExperiment.Assisted)] [TestCase(FlightExperiment.Momentum)]
        public void ReleaseRecoversNeutralWithoutStoppingForwardFlight(FlightExperiment mode)
        {
            profile.experiment = mode;
            var state = State;
            for (int i = 0; i < 100; i++) state = FlightSimulation.Step(state, new FlightIntent { steer = Vector2.one, flap = 1 }, profile, 0.02f);
            for (int i = 0; i < 500; i++) state = FlightSimulation.Step(state, default, profile, 0.02f);
            Assert.That(Mathf.Abs(state.pitch), Is.LessThan(0.1));
            Assert.That(Mathf.Abs(state.bank), Is.LessThan(0.1));
            Assert.That(Mathf.Abs(state.yawRate), Is.LessThan(0.1));
            Assert.That(state.velocity.magnitude, Is.GreaterThan(profile.cruiseSpeed * 0.95f));
        }
        [Test] public void MomentumExperimentRetainsMoreTurnAfterRelease()
        {
            var assisted = State; var momentum = State;
            for (int i = 0; i < 150; i++)
            {
                var intent = new FlightIntent { steer = i < 100 ? Vector2.right : Vector2.zero };
                profile.experiment = FlightExperiment.Assisted; profile.response = 5; profile.velocityResponse = 7;
                assisted = FlightSimulation.Step(assisted, intent, profile, 0.02f);
                profile.experiment = FlightExperiment.Momentum; profile.response = 1.5f; profile.velocityResponse = 1.8f;
                momentum = FlightSimulation.Step(momentum, intent, profile, 0.02f);
            }
            Assert.That(momentum.yawRate, Is.GreaterThan(assisted.yawRate + 5));
        }
        [TestCase(30)] [TestCase(60)] [TestCase(120)]
        public void FixedStepScheduleGivesSameTrajectoryAcrossRenderRates(int renderRate)
        {
            var reference = State; var scheduled = State;
            var intent = new FlightIntent { steer = new Vector2(0.4f, 0.3f), flap = 0.6f };
            for (int i = 0; i < 500; i++) reference = FlightSimulation.Step(reference, intent, profile, 0.02f);
            double accumulator = 0;
            for (int frame = 0; frame < renderRate * 10; frame++)
            {
                accumulator += 1.0 / renderRate;
                while (accumulator + 1e-9 >= 0.02) { scheduled = FlightSimulation.Step(scheduled, intent, profile, 0.02f); accumulator -= 0.02; }
            }
            Assert.That(Vector3.Distance(reference.position, scheduled.position), Is.LessThan(0.001f));
        }
        [Test] public void SwitchingProfileDoesNotTeleportBird()
        {
            var state = State;
            profile.experiment = FlightExperiment.Momentum;
            var next = FlightSimulation.Step(state, default, profile, 0.02f);
            Assert.That(Vector3.Distance(state.position, next.position), Is.LessThan(1));
            Assert.That(next.velocity.magnitude, Is.GreaterThan(5));
        }
    }
}
