﻿using UnityEngine;

using StressLevelZero.AI;
using StressLevelZero.Combat;

using PuppetMasta;

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
