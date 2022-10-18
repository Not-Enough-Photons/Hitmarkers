using UnityEngine;
using SLZ.AI;

namespace NEP.Hitmarkers
{
    public static class HitDirector
    {
        public static System.Action<Vector3> OnHit;
        public static System.Action<AIBrain> OnKill;

        public static void Initialize()
        {
            OnHit += OnProjectileHit;
        }

        public static void OnProjectileHit(Vector3 position)
        {
            HitmarkerManager.SpawnMarker(position);
        }

        public static void OnAIDeath(AIBrain brain)
        {
            TriggerRefProxy proxy = brain.GetComponent<TriggerRefProxy>();
            HitmarkerManager.SpawnMarker(proxy.targetHead.position);
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
                HitDirector.OnHit?.Invoke(world);
            }));
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(AIBrain))]
    [HarmonyLib.HarmonyPatch(nameof(AIBrain.Awake))]
    public static class AIBrainPatch
    {
        public static void Postfix(AIBrain __instance)
        {
            __instance.onDeathDelegate += new System.Action(() => HitDirector.OnKill?.Invoke(__instance));
        }
    }
}
