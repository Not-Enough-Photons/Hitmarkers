using UnityEngine;

using SLZ.AI;
using SLZ.Combat;

using PuppetMasta;
using System.Linq;
using System.Collections.Generic;
using static Il2CppSystem.Globalization.CultureInfo;
using MelonLoader;
using BoneLib;
using SLZ.Rig;
using SLZ.SFX;

namespace NEP.Hitmarkers
{
    public static class HitDirector
    {
        public static System.Action<HitData> OnHit;
        public static System.Action<PuppetMaster> OnKill;

        public static RigManager lastHitManager;

        public static void Initialize()
        {
            OnHit += OnProjectileHit;
            PuppetMaster.add_OnDeathStatsEvent(new System.Action<PuppetMaster>(OnPuppetDeath));
        }

        public static bool EvaluateHit(HitData data)
        {
            if(data.projectile._proxy.root.name != "[RigManager (Blank)]")
            {
                return false;
            }

            // Makes it so any NPC with a gun can't spawn hitmarkers
            if (data.projectile._proxy.triggerType != TriggerRefProxy.TriggerType.Player)
            {
                return true;
            }

            if (data.brain != null)
            {
                if (data.brain.isDead)
                {
                    return false;
                }
            }

            var hitObject = data.collider.gameObject;

            bool hitEnemy = hitObject.layer == LayerMask.NameToLayer("EnemyColliders");

            if (hitEnemy)
            {
                return true;
            }

            return false;
        }

        public static void OnProjectileHit(HitData data)
        {
            if (EvaluateHit(data))
            {
                HitmarkerManager.Instance.SpawnMarker(data.worldHit);
            }
        }

        public static void OnPuppetDeath(PuppetMaster puppet)
        {
            BehaviourBaseNav bbn = puppet.behaviours?.FirstOrDefault()?.TryCast<BehaviourBaseNav>();
            if (bbn == null) return;

            OnKill?.Invoke(puppet);
            HitmarkerManager.Instance.SpawnMarker(bbn.eyeTran.position, true);
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(Projectile))]
    [HarmonyLib.HarmonyPatch(nameof(Projectile.Awake))]
    public static class ProjectilePatch
    {
        public static void Postfix(Projectile __instance)
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

    [HarmonyLib.HarmonyPatch(typeof(PuppetMaster))]
    [HarmonyLib.HarmonyPatch(nameof(PuppetMaster.Awake))]
    public static class PuppetMasterPatch
    {
        public static void Postfix(PuppetMaster __instance)
        {
            PuppetMaster.StateSettings settings = __instance.stateSettings; // structs are value types
            settings.killDuration = 0;

            __instance.stateSettings = settings;
        }
    }
}
