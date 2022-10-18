using UnityEngine;

using SLZ.AI;
using SLZ.Combat;
using PuppetMasta;

using System.Collections.Generic;
using System.Linq;

namespace NEP.Hitmarkers
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class HitmarkerManager : MonoBehaviour
    {
        public HitmarkerManager(System.IntPtr ptr) : base(ptr) { }

        private List<Hitmarker> _hitmarkers;
        private List<Hitmarker> _finishers;

        private int _markerCount = 64;

        private Transform _poolHitmarker;
        private Transform _poolFinisher;

        private void Awake()
        {
            BuildPools();

            _hitmarkers = new List<Hitmarker>(_markerCount);
            _finishers = new List<Hitmarker>(_markerCount);

            for(int i = 0; i < _markerCount; i++)
            {
                _hitmarkers[i] = BuildHitmarker(isFinisher: false);
                _finishers[i] = BuildHitmarker(isFinisher: true);
            }
        }

        private void BuildPools()
        {
            _poolHitmarker = new GameObject("Hitmarker Pool").transform;
            _poolFinisher = new GameObject("Finisher Pool").transform;

            _poolHitmarker.transform.SetParent(transform);
            _poolFinisher.transform.SetParent(transform);
        }

        private Hitmarker BuildHitmarker(bool isFinisher)
        {
            string name = !isFinisher ? "Hitmarker" : "Finisher";
            GameObject marker = new GameObject(name);

            marker.hideFlags = HideFlags.DontUnloadUnusedAsset;

            marker.transform.parent = !isFinisher ? _poolHitmarker : _poolFinisher;

            return marker.AddComponent<Hitmarker>();
        }

        private Hitmarker GetInactiveMarker(bool finisher)
        {
            var list = !finisher ? _hitmarkers : _finishers;

            for(int i = 0; i < list.Count; i++)
            {
                if (!list[i].gameObject.active)
                {
                    return list[i];
                }
            }

            return null;
        }

        public static void SpawnMarker(Vector3 position, bool finisher = false)
        {
            Hitmarker marker = GetInactiveMarker(finisher);
            marker.transform.position = position;
            marker.gameObject.SetActive(true);
        }
    }
}
