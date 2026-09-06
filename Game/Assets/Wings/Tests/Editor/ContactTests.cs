using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Wings.Tests
{
    public sealed class ContactTests
    {
        readonly List<Object> owned = new List<Object>();
        readonly Vector3 origin = new Vector3(5000, 100, 5000);
        GameObject Node(string name, Vector3 point)
        {
            var node = new GameObject(name); owned.Add(node); node.transform.position = point; return node;
        }
        GameObject Wall(Vector3 point)
        {
            var wall = Node("Obstacle", point); wall.AddComponent<BoxCollider>().size = new Vector3(10, 10, 0.1f);
            Physics.SyncTransforms(); return wall;
        }
        BirdMotor Bird(out Perch perch)
        {
            var profile = ScriptableObject.CreateInstance<FlightProfile>(); owned.Add(profile);
            var node = Node("Bird", origin);
            var bird = node.AddComponent<BirdMotor>(); bird.profile = profile; bird.input = node.AddComponent<FlightInput>();
            bird.origin = origin; bird.ResetFlight();
            perch = Node("Perch", origin + Vector3.forward * 10).AddComponent<Perch>();
            bird.perches = new[] { perch }; bird.startingPerch = perch; bird.SelectedPerch = perch; return bird;
        }
        [TearDown] public void Cleanup() { for (int i = owned.Count - 1; i >= 0; i--) Object.DestroyImmediate(owned[i]); owned.Clear(); }

        [Test] public void SweptContactStopsFastBirdAtThinWallAndDeflects()
        {
            Wall(origin + Vector3.forward * 5);
            var before = new FlightState { position = origin, velocity = Vector3.forward * 100 };
            var desired = before; desired.position += Vector3.forward * 20;
            var result = FlightContact.Move(before, desired, out bool hit, out bool water);
            Assert.That(hit, Is.True); Assert.That(water, Is.False);
            Assert.That(result.position.z, Is.LessThan(origin.z + 5));
            Assert.That(result.velocity.z, Is.LessThan(0)); Assert.That(FlightContact.IsClear(result.position), Is.True);
        }
        [Test] public void WaterIsIdentifiedAndCameraRemainsInFrontOfObstacle()
        {
            Wall(origin + Vector3.forward * 5).AddComponent<WaterSurface>();
            var before = new FlightState { position = origin, velocity = Vector3.forward * 20 };
            var desired = before; desired.position += Vector3.forward * 10;
            FlightContact.Move(before, desired, out _, out bool water);
            Assert.That(water, Is.True);
            var camera = FlightCamera.ClearCameraPosition(origin, desired.position);
            Assert.That(camera.z, Is.LessThan(origin.z + 4.7f));
        }
        [Test] public void LandingCanCancelThenCompleteAndTakeOff()
        {
            var bird = Bird(out var perch);
            bird.RequestPerchAction(); Assert.That(bird.CurrentActivity, Is.EqualTo(BirdMotor.Activity.Approach));
            bird.AdvanceAssistance(0.5f); var position = bird.State.position;
            bird.RequestPerchAction(); Assert.That(bird.CurrentActivity, Is.EqualTo(BirdMotor.Activity.Flying));
            Assert.That(bird.State.position, Is.EqualTo(position)); Assert.That(bird.LastSafePerch, Is.Null);
            bird.ResetFlight(); // A fresh fly-in after cancellation, not a hover/reversal at point-blank range.
            bird.RequestLanding(perch); bird.AdvanceAssistance(3);
            Assert.That(bird.CurrentActivity, Is.EqualTo(BirdMotor.Activity.Perched)); Assert.That(bird.LastSafePerch, Is.EqualTo(perch));
            Assert.That(bird.State.velocity, Is.EqualTo(Vector3.zero));
            bird.RequestPerchAction(); bird.AdvanceAssistance(2);
            Assert.That(bird.CurrentActivity, Is.EqualTo(BirdMotor.Activity.Flying));
            Assert.That(bird.State.position.z, Is.GreaterThan(perch.Point.z + 5));
        }
        [Test] public void BlockedAndDistantRequestsLeaveFlightUnchanged()
        {
            var bird = Bird(out var perch); var initial = bird.State;
            Wall(origin + Vector3.forward * 5); bird.RequestPerchAction();
            Assert.That(bird.CurrentActivity, Is.EqualTo(BirdMotor.Activity.Flying)); Assert.That(bird.State.position, Is.EqualTo(initial.position));
            perch.transform.position = origin + Vector3.right * 100; bird.RequestPerchAction();
            Assert.That(bird.CurrentActivity, Is.EqualTo(BirdMotor.Activity.Flying));
        }
        [Test] public void NewObstructionCancelsApproachAndBlocksTakeoff()
        {
            var bird = Bird(out var perch); bird.RequestPerchAction();
            var wall = Wall(origin + Vector3.forward * 5); bird.AdvanceAssistance(3);
            Assert.That(bird.CurrentActivity, Is.EqualTo(BirdMotor.Activity.Flying));
            wall.SetActive(false); Physics.SyncTransforms(); bird.RequestLanding(perch); bird.AdvanceAssistance(3);
            Wall(bird.DeparturePosition(0.5f,bird.State.heading,perch.Point)); bird.RequestPerchAction();
            Assert.That(bird.CurrentActivity, Is.EqualTo(BirdMotor.Activity.Perched));
        }
        [Test] public void RecoveryValidatesSavedPerchAndFallsBackToStartingPerch()
        {
            var bird = Bird(out var perch); bird.RequestLanding(perch); bird.AdvanceAssistance(3);
            var fallback = Node("Fallback", origin + Vector3.right * 20).AddComponent<Perch>(); bird.startingPerch = fallback;
            Wall(perch.Point); bird.Recover("Test recovery");
            Assert.That(bird.LastSafePerch, Is.EqualTo(fallback)); Assert.That(bird.State.position, Is.EqualTo(fallback.Point));
            fallback.gameObject.SetActive(false); bird.Recover("Emergency recovery");
            Assert.That(bird.CurrentActivity, Is.EqualTo(BirdMotor.Activity.Flying)); Assert.That(FlightContact.IsClear(bird.State.position), Is.True);
        }
    }
}
