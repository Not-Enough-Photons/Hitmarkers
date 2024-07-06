using UnityEngine;

using Il2CppSLZ.Marrow.AI;
using Il2CppSLZ.Combat;

using Il2CppSLZ.Marrow.PuppetMasta;

namespace NEP.Hitmarkers
{
    public struct HitData
    {
        public Projectile projectile;
        public Vector3 worldHit;
        public Collider collider;

        public BehaviourBaseNav behaviour;
        public AIBrain brain;
    }
}
