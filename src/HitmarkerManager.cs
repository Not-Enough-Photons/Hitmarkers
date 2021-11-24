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

        public static List<Hitmarker> hitmarkerPool;

        public List<AIBrain> deadNPCs;

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
            hitmarkerPool = new List<Hitmarker>();
            deadNPCs = new List<AIBrain>();

            Transform hitmarkerPoolChild = new GameObject("Hitmarker Pool").transform;
            hitmarkerPoolChild.SetParent(transform);

            for(int i = 0; i < hitmarkerPoolCount; i++)
            {
                Hitmarker hitmarker = new GameObject($"Hitmarker {i}").AddComponent<Hitmarker>();
                hitmarker.gameObject.hideFlags = HideFlags.DontUnloadUnusedAsset;
                hitmarker.transform.SetParent(hitmarkerPoolChild);
                hitmarkerPool.Add(hitmarker);
            }
        }

        private void Update()
        {
            for (int i = 0; i < deadNPCs.Count; i++)
            {
                if (deadNPCs[i].behaviour.health.cur_hp > 0f || deadNPCs[i] == null)
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

            if(navBehaviour.puppetMaster.isDead) { return; }

            SpawnHitmarker(brain, impactWorld);
        }

        private static void SpawnHitmarker(AIBrain brain, Vector3 position)
        {
            Hitmarker hitmarker = hitmarkerPool.FirstOrDefault((marker) => !marker.gameObject.active);

            lastHitmarker = hitmarker;

            SetupHitmarker(hitmarker, position);

            hitmarker.gameObject.SetActive(true);
        }

        private static void SetupHitmarker(Hitmarker hitmarker, Vector3 position)
        {
            AudioClip hitAudioClip = hitmarker.hitAudio[Random.Range(0, hitmarker.hitAudio.Count)];
            AudioClip hitFinisherClip = hitmarker.hitFinisherAudio[Random.Range(0, hitmarker.hitFinisherAudio.Count)];

            hitmarker.transform.position = position;

            if (hitmarker.finisherHitmarker)
            {
                hitmarker.SetClip(hitFinisherClip);
                hitmarker.animator.Play("hm_finisher_appear1");
            }
            else
            {
                hitmarker.SetClip(hitAudioClip);
            }
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
            HitmarkerManager._instance.deadNPCs.Add(__instance.GetComponentInParent<AIBrain>());
            HitmarkerManager.lastHitmarker.finisherHitmarker = true;
        }
    }
}
