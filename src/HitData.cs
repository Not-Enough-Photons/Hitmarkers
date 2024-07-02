using UnityEngine;

using Il2CppSLZ.Marrow.AI;
using Il2CppSLZ.Combat;

using Il2CppPuppetMasta;

namespace NEP.Hitmarkers
{
    public struct HitData
    {
        public Projectile projectile;
        public Vector3 worldHit;
        public Collider collider;

        public BehaviourPowerLegs behaviour;
        public AIBrain brain;
    }
}
