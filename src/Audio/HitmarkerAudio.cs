using UnityEngine;

using System.Collections.Generic;
using System.Linq;

namespace NEP.Hitmarkers.Audio
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class HitmarkerAudio : MonoBehaviour
    {
        public HitmarkerAudio(System.IntPtr ptr) : base(ptr) { }

        public static HitmarkerAudio Instance;

        public static AudioClip[] HitAudio;
        public static AudioClip[] FinisherAudio;

        private static List<GameObject> pooledAudioObjects;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }

            DontDestroyOnLoad(Instance);

            pooledAudioObjects = new List<GameObject>();

            GameObject listObj = new GameObject("Pooled Audio List");
            listObj.transform.parent = transform;

            for (int i = 0; i < 64; i++)
            {
                GameObject pooledAudio = new GameObject("Poolee Audio");
                pooledAudio.transform.parent = listObj.transform;

                AudioSource source = pooledAudio.AddComponent<AudioSource>();
                source.playOnAwake = true;
                source.volume = 5f;

                pooledAudio.AddComponent<PooledAudio>();
                pooledAudio.SetActive(false);
                pooledAudioObjects.Add(pooledAudio);
            }

            HitAudio = Data.DataManager.HitClips;
            FinisherAudio = Data.DataManager.FinisherClips;
        }

        public static void PlayAtPoint(AudioClip clip, Vector3 position)
        {
            GameObject first = pooledAudioObjects.FirstOrDefault((inactive) => !inactive.activeInHierarchy);
            AudioSource src = first.GetComponent<AudioSource>();

            if (first != null)
            {
                src.volume = Data.Options.HitmarkerSFX / 100f;
                src.pitch = Data.Options.HitmarkerPitch;
                src.clip = clip;
                first.transform.position = position;
                first.SetActive(true);
            }
        }
    }
}