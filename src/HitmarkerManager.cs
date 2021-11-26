using UnityEngine;

using StressLevelZero.AI;
using StressLevelZero.Combat;
using PuppetMasta;

using System.Collections.Generic;
using System.Linq;

namespace NEP.Hitmarkers
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class HitmarkerManager : MonoBehaviour
    {
        public HitmarkerManager(System.IntPtr ptr) : base(ptr) { }

        public static HitmarkerManager _instance;

        public static List<Hitmarker> regularHitmarkerPool;
        public static List<Hitmarker> finisherHitmarkerPool;

        public static List<BehaviourBaseNav> deadNPCs;

        public bool startKill { get; set; }

        private int hitmarkerPoolCount = 32;

        public static Hitmarker lastHitmarker { get; private set; }

        private void Awake()
        {
            if(_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(_instance.gameObject);
            }

            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            HitmarkerManager.regularHitmarkerPool = new List<Hitmarker>();
            HitmarkerManager.finisherHitmarkerPool = new List<Hitmarker>();
            deadNPCs = new List<BehaviourBaseNav>();

            Transform regularHitmarkerPool = new GameObject("Regular Hitmarker Pool").transform;
            Transform finisherHitmarkerPool = new GameObject("Finisher Hitmarker Pool").transform;
            regularHitmarkerPool.SetParent(transform);
            finisherHitmarkerPool.SetParent(transform);

            for(int i = 0; i < hitmarkerPoolCount; i++)
            {
                Hitmarker hitmarker = new GameObject($"Regular Hitmarker {i}").AddComponent<Hitmarker>();
                hitmarker.gameObject.hideFlags = HideFlags.DontUnloadUnusedAsset;
                hitmarker.transform.SetParent(regularHitmarkerPool);
                HitmarkerManager.regularHitmarkerPool.Add(hitmarker);
            }

            for(int i = 0; i < hitmarkerPoolCount; i++)
            {
                Hitmarker hitmarker = new GameObject($"Finisher Hitmarker {i}").AddComponent<Hitmarker>();
                hitmarker.gameObject.hideFlags = HideFlags.DontUnloadUnusedAsset;
                hitmarker.transform.SetParent(finisherHitmarkerPool);
                hitmarker.UseFinisherHitmarker(true);
                HitmarkerManager.finisherHitmarkerPool.Add(hitmarker);
            }
        }

        private void Update()
        {
            for (int i = 0; i < deadNPCs.Count; i++)
            {
                if (deadNPCs[i].health.cur_hp > 0f || deadNPCs[i] == null)
                {
                    deadNPCs.RemoveAt(i);
                }
            }
        }

        public static void OnProjectileCollision(TriggerRefProxy playerProxy, Collider collider, Vector3 impactWorld, Vector3 impactNormal)
        {
            if(collider.gameObject.layer != 12 || playerProxy.triggerType != TriggerRefProxy.TriggerType.Player) { return; }

            AIBrain brain = collider.GetComponentInParent<AIBrain>();
            BehaviourBaseNav navBehaviour = brain.behaviour;

            if(navBehaviour.puppetMaster.isDead || navBehaviour == deadNPCs.FirstOrDefault((npc) => npc == navBehaviour)) { return; }

            SpawnHitmarker(navBehaviour.puppetMaster.isKilling, impactWorld);
        }

        public static void SpawnHitmarker(bool isFinisher, Vector3 position)
        {
            Hitmarker hitmarker = isFinisher ? finisherHitmarkerPool.FirstOrDefault((marker) => !marker.gameObject.active) : regularHitmarkerPool.FirstOrDefault((marker) => !marker.gameObject.active);

            lastHitmarker = hitmarker;

            SetupHitmarker(hitmarker, position);

            hitmarker.gameObject.SetActive(true);
        }

        private static void SetupHitmarker(Hitmarker hitmarker, Vector3 position)
        {
            hitmarker.transform.position = position;
        }

        private static bool EvaluateHealth(SubBehaviourHealth healthModule)
        {
            return healthModule.cur_hp <= 0f;
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(Projectile))]
    [HarmonyLib.HarmonyPatch(nameof(Projectile.Awake))]
    public static class ProjectilePatch
    {
        public static void Prefix(Projectile __instance)
        {
            __instance.onCollision.AddListener(
                new System.Action<Collider, Vector3, Vector3>
                (((Collider collider, Vector3 world, Vector3 normal)
                =>
                {
                    HitmarkerManager.OnProjectileCollision(__instance._proxy, collider, world, normal);
                })));
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(BehaviourBaseNav))]
    [HarmonyLib.HarmonyPatch(nameof(BehaviourBaseNav.KillStart))]
    public static class BehaviourPatch
    {
        public static void Postfix(BehaviourBaseNav __instance)
        {
            HitmarkerManager.deadNPCs.Add(__instance);
            HitmarkerManager.lastHitmarker.gameObject.SetActive(false);
            HitmarkerManager.SpawnHitmarker(true, HitmarkerManager.lastHitmarker.transform.position);
        }
    }
}
