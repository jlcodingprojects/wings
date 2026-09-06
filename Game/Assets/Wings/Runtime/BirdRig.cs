using System;
using UnityEngine;

namespace Wings
{
    // Explicit bindings allow a skinned FBX/Generic Animator to replace the prototype geometry.
    public sealed class BirdRig : MonoBehaviour
    {
        [Serializable] public sealed class Wing
        { public Transform shoulder, elbow, wrist; }
        [Serializable] public sealed class Leg
        { public Transform hip, knee, ankle, foot; public Transform[] toes; }
        public Transform body, head, tail;
        public Transform[] tailFeathers;
        public Wing leftWing, rightWing;
        public Leg leftLeg, rightLeg;
        public Animator authoredAnimator;
        public bool useAuthoredAnimation;
        public bool IsComplete => body && head && tail && Valid(leftWing) && Valid(rightWing) && Valid(leftLeg) && Valid(rightLeg);
        static bool Valid(Wing w) => w != null && w.shoulder && w.elbow && w.wrist;
        static bool Valid(Leg l) => l != null && l.hip && l.knee && l.ankle && l.foot && l.toes != null && l.toes.Length >= 3;
    }
}
