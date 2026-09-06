using UnityEngine;

namespace Wings
{
    public sealed class Perch : MonoBehaviour
    {
        public string displayName = "Perch";
        public float requestRadius = 55;
        public float footHeight = 0.62f;
        public Vector3 Point => transform.position;
        public Vector3 FootPoint => Point - Vector3.up * footHeight;
        public bool IsUsable => isActiveAndEnabled && FlightContact.IsClear(Point);
        public bool CanApproach(Vector3 from) => IsUsable && Vector3.Distance(from, Point) <= requestRadius && FlightContact.PathClear(from, Point);
    }
}
