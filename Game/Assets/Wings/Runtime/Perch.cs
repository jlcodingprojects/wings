using UnityEngine;

namespace Wings
{
    public sealed class Perch : MonoBehaviour
    {
        public string displayName = "Perch";
        public float requestRadius = 38;
        public Vector3 Point => transform.position;
        public Vector3 LaunchPoint => Point + transform.forward * 12 + Vector3.up * 5;
        public bool IsUsable => isActiveAndEnabled && FlightContact.IsClear(Point);
        public bool CanApproach(Vector3 from) => IsUsable && Vector3.Distance(from, Point) <= requestRadius && FlightContact.PathClear(from, Point);
        public bool CanTakeOff => IsUsable && FlightContact.IsClear(LaunchPoint) && FlightContact.PathClear(Point, LaunchPoint);
    }
}
