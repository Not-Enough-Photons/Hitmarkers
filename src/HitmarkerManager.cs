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

        public static HitmarkerManager Instance;

        private List<Hitmarker> _hitmarkers;
        private List<Hitmarker> _finishers;

        private int _markerCount = 64;

        private Transform _poolHitmarker;
        private Transform _poolFinisher;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }

            DontDestroyOnLoad(Instance);

            BuildPools();

            _hitmarkers = new List<Hitmarker>();
            _finishers = new List<Hitmarker>();

            for(int i = 0; i < _markerCount; i++)
            {
                _hitmarkers.Add(BuildHitmarker(isFinisher: false));
                _finishers.Add(BuildHitmarker(isFinisher: true));
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
            GameObject marker = GameObject.Instantiate(Data.DataManager.GetGameObject("Hitmarker"));

            marker.name = name;
            marker.hideFlags = HideFlags.DontUnloadUnusedAsset;

            marker.transform.parent = !isFinisher ? _poolHitmarker : _poolFinisher;

            // stops the loudest noise i've ever heard
            marker.gameObject.SetActive(false);

            return marker.AddComponent<Hitmarker>();
        }

        private Hitmarker GetInactiveMarker(bool finisher)
        {
            var list = !finisher ? _hitmarkers : _finishers;

            for(int i = 0; i < list.Count; i++)
            {
                if (!list[i].gameObject.activeInHierarchy)
                {
                    return list[i];
                }
            }

            return null;
        }

        public void SpawnMarker(Vector3 position, bool finisher = false)
        {
            Hitmarker marker = GetInactiveMarker(finisher);
            marker.IsFinisher = finisher;
            marker.transform.position = position;
            marker.gameObject.SetActive(true);
        }
    }
}
