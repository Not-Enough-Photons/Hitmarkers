using UnityEngine;

using System.Collections.Generic;
using System.Linq;

namespace NEP.Hitmarkers.Audio
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class HitmarkerAudio : MonoBehaviour
    {
        public HitmarkerAudio(System.IntPtr ptr) : base(ptr) { }

        public static List<AudioClip> hitAudio { get; private set; }
        public static List<AudioClip> hitFinisherAudio { get; private set; }

        private static List<GameObject> pooledAudioObjects;

        private void Awake()
        {
            pooledAudioObjects = new List<GameObject>();

            hitAudio = new List<AudioClip>();
            hitFinisherAudio = new List<AudioClip>();

            GameObject listObj = new GameObject("Pooled Audio List");
            listObj.transform.parent = transform;

            for(int i = 0; i < 64; i++)
            {
                GameObject pooledAudio = new GameObject("Poolee Audio");
                pooledAudio.transform.parent = listObj.transform;

                AudioSource source = pooledAudio.AddComponent<AudioSource>();
                source.playOnAwake = true;
                source.volume = HitmarkerManager._instance.hitmarkerAudio;

                pooledAudio.AddComponent<PooledAudio>();
                pooledAudio.SetActive(false);
                pooledAudioObjects.Add(pooledAudio);
            }

            hitAudio = AudioUtilities.GetClips(AudioUtilities.hitmarkerDir);
            hitFinisherAudio = AudioUtilities.GetClips(AudioUtilities.hitmarkerFinisherDir);
        }

        public static void PlayAtPoint(AudioClip clip, Vector3 position)
        {
            GameObject first = pooledAudioObjects.FirstOrDefault((inactive) => !inactive.activeInHierarchy);
            AudioSource src = first.GetComponent<AudioSource>();
             
            if(first != null)
            {
                src.clip = clip;
                first.transform.position = position;
                first.SetActive(true);
            }
        }
    }
}
