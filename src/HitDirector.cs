﻿using UnityEngine;

using SLZ.AI;
using SLZ.Combat;

using PuppetMasta;

namespace NEP.Hitmarkers
{
    public static class HitDirector
    {
        public static System.Action<HitData> OnHit;
        public static System.Action<AIBrain> OnKill;

        public static void Initialize()
        {
            OnHit += OnProjectileHit;
        }

        public static void OnProjectileHit(HitData data)
        {
            // Makes it so any NPC with a gun can't spawn hitmarkers
            if(data.projectile._proxy.triggerType != TriggerRefProxy.TriggerType.Player)
            {
                return;
            }

            if(data.collider.gameObject.layer != LayerMask.NameToLayer("EnemyColliders"))
            {
                return;
            }

            if(data.brain != null)
            {
                if (data.brain.isDead)
                {
                    return;
                }
            }

            HitmarkerManager.Instance.SpawnMarker(data.worldHit);
        }

        public static void OnAIDeath(AIBrain brain)
        {
            HitmarkerManager.Instance.SpawnMarker(brain.behaviour.eyeTran.position, true);
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(SLZ.Combat.Projectile))]
    [HarmonyLib.HarmonyPatch(nameof(SLZ.Combat.Projectile.Awake))]
    public static class ProjectilePatch
    {
        public static void Postfix(SLZ.Combat.Projectile __instance)
        {
            __instance.onCollision.AddListener(new System.Action<Collider, Vector3, Vector3>((hitCol, world, normal) =>
            {
                var hitData = new HitData()
                {
                    projectile = __instance,
                    worldHit = world,
                    collider = hitCol,
                    brain = hitCol.GetComponentInParent<AIBrain>()
                };

                HitDirector.OnHit?.Invoke(hitData);
            }));
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(AIBrain))]
    [HarmonyLib.HarmonyPatch(nameof(AIBrain.Awake))]
    public static class AIBrainPatch
    {
        public static void Postfix(AIBrain __instance)
        {
            __instance.behaviour.OnDeath.AddListener(new System.Action(() => HitDirector.OnAIDeath(__instance)));
        }
    }
}