using NUnit.Framework;
using UnityEngine;

namespace Wings.Tests
{
    public sealed class PerchMotionTests
    {
        [TestCase(8)] [TestCase(14)] [TestCase(24)]
        public void AcquisitionPreservesVelocityAndBrakesIntoForwardContact(float speed)
        {
            var state=new FlightState { position=new Vector3(0,20,0),velocity=Vector3.forward*speed };
            var path=new LandingTrajectory(state,new Vector3(0,19,25));
            Assert.That(Vector3.Distance(path.Velocity(0),state.velocity),Is.LessThan(0.001f));
            float lastZ=state.position.z;
            for(int i=1;i<=100;i++)
            {
                var point=path.Position(i/100f);
                Assert.That(point.z,Is.GreaterThan(lastZ)); lastZ=point.z;
            }
            Assert.That(path.Velocity(0.9f).magnitude,Is.LessThan(speed*0.5f));
            Assert.That(path.Velocity(1).z,Is.EqualTo(0.65f).Within(0.001f));
            Assert.That(path.Position(1),Is.EqualTo(path.end));
        }
        [Test] public void AimingSelectsThePerchAheadRatherThanANearerPerchBehind()
        {
            var a=new GameObject("Ahead").AddComponent<Perch>(); var b=new GameObject("Behind").AddComponent<Perch>();
            try
            {
                var state=new FlightState {position=new Vector3(5000,100,5000),velocity=Vector3.forward*14};
                a.transform.position=state.position+Vector3.forward*25; b.transform.position=state.position-Vector3.forward*6;
                Assert.That(PerchTargeting.Select(new[]{b,a},state,new Ray(state.position,Vector3.forward)),Is.EqualTo(a));
                Assert.That(PerchTargeting.Select(new[]{a,b},state,new Ray(state.position,Vector3.right)),Is.Null);
                a.transform.position=state.position+Vector3.up*10;
                Assert.That(PerchTargeting.CanLand(a,state),Is.False);
                a.transform.position=state.position+Vector3.forward*5;
                state.velocity=Vector3.forward*24;
                Assert.That(PerchTargeting.CanLand(a,state),Is.False,"No last-moment reversal when there is insufficient forward braking room.");
            }
            finally { Object.DestroyImmediate(a.gameObject); Object.DestroyImmediate(b.gameObject); }
        }
    }
}
