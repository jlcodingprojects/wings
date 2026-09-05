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

        [TestCase(FlightExperiment.Assisted)] [TestCase(FlightExperiment.Momentum)]
        public void DiveIsBoundedAndDoesNotIncreaseTurningAuthority(FlightExperiment mode)
        {
            profile.experiment = mode;
            var diving = State; var level = State;
            var tuning = new FlightTuning(profile, FlightContext.Shared);
            for (int i = 0; i < 400; i++)
            {
                diving = FlightSimulation.Step(diving, new FlightIntent { steer = new Vector2(0.3f, -0.8f), flap = 1 }, tuning, 0.02f);
                level = FlightSimulation.Step(level, new FlightIntent { steer = new Vector2(0.3f, 0), flap = 1 }, tuning, 0.02f);
                Assert.That(diving.velocity.magnitude, Is.LessThanOrEqualTo(tuning.MaximumSpeed + 0.001f));
            }
            Assert.That(diving.mode, Is.EqualTo(FlightMode.Dive));
            Assert.That(diving.velocity.magnitude, Is.GreaterThan(level.velocity.magnitude + 2));
            Assert.That(diving.yawRate, Is.EqualTo(level.yawRate).Within(0.001f));
            for (int i = 0; i < 500; i++) diving = FlightSimulation.Step(diving, default, tuning, 0.02f);
            Assert.That(diving.mode, Is.EqualTo(FlightMode.Glide));
            Assert.That(diving.velocity.magnitude, Is.EqualTo(profile.cruiseSpeed).Within(0.05f));
        }

        [Test] public void ContextResolutionIsIsolatedAndDoesNotMutateSharedDefaults()
        {
            var normal = new FlightTuning(profile, FlightContext.Shared);
            var influenced = new FlightTuning(profile, new FlightContext(0.8f, 1.2f));
            Assert.That(influenced.cruiseSpeed, Is.EqualTo(normal.cruiseSpeed * 0.8f).Within(0.001f));
            Assert.That(influenced.turnRate, Is.EqualTo(normal.turnRate * 1.2f).Within(0.001f));
            Assert.That(profile.cruiseSpeed, Is.EqualTo(normal.cruiseSpeed));
            profile.cruiseSpeed += 5;
            Assert.That(normal.cruiseSpeed, Is.EqualTo(14));
            Assert.That(new FlightTuning(profile, default).cruiseSpeed, Is.EqualTo(profile.cruiseSpeed));
        }

        [TestCase(30)] [TestCase(60)] [TestCase(120)]
        public void DiveFlapAndRecoverySequenceMatchesAtDifferentRenderSchedules(int fps)
        {
            FlightIntent Intent(int step) => step < 100 ? new FlightIntent { flap = 1 } : step < 220 ? new FlightIntent { steer = new Vector2(0.3f, -0.8f) } : default;
            var expected = State; var actual = State;
            for (int i = 0; i < 500; i++) expected = FlightSimulation.Step(expected, Intent(i), profile, 0.02f);
            double accumulator = 0; int steps = 0;
            for (int frame = 0; frame < fps * 10; frame++)
            {
                accumulator += 1.0 / fps;
                while (accumulator + 1e-9 >= 0.02) { actual = FlightSimulation.Step(actual, Intent(steps++), profile, 0.02f); accumulator -= 0.02; }
            }
            Assert.That(Vector3.Distance(expected.position, actual.position), Is.LessThan(0.001f));
            Assert.That(actual.mode, Is.EqualTo(FlightMode.Glide));
        }
    }
}
