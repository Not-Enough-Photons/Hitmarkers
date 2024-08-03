using System.Linq;

using UnityEngine;

using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.AI;
using Il2CppSLZ.Marrow.PuppetMasta;
using Il2CppSLZ.Marrow.Interaction;

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
            TriggerRefProxy proxy = data.projectile._proxy;
            
            // TODO:
            // Check if the player is the first ever rig to get made,
            // since in patch 4+ the rig manager is a pooled object

            // Makes it so any NPC with a gun can't spawn hitmarkers
            if (proxy.triggerType != TriggerRefProxy.TriggerType.Player)
            {
                return true;
            }

            // Has a brain?
            if (data.brain != null)
            {
                // Is the brain already dead?
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

            bool hitProxy = hitObject.GetComponent<HitmarkerProxy>();

            if (hitProxy)
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
                MarrowBody cachedHit = MarrowBody.Cache.Get(hitCol.gameObject);

                if (cachedHit == null)
                {
                    return;
                }

                MarrowEntity entity = cachedHit.Entity;

                AIBrain brain = entity.GetComponent<AIBrain>();

                var hitData = new HitData()
                {
                    projectile = __instance,
                    worldHit = world,
                    collider = hitCol,
                    brain = brain
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
