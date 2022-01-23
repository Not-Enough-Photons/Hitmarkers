using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using NEP.Hitmarkers.Utilities;

namespace NEP.Hitmarkers
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class Hitmarker : MonoBehaviour
    {
        public Hitmarker(System.IntPtr ptr) : base(ptr) { }

        public Animator animator { get; private set; }

        public List<AudioClip> hitAudio { get; private set; }
        public List<AudioClip> hitFinisherAudio { get; private set; }

        public AudioSource source { get; private set; }
        public GameObject deathSkullGO { get; private set; }

        public bool finisherHitmarker;

        private Texture2D crossTexture;
        private Texture2D skullTexture;

        private AnimationClip hm_appear1;
        private AnimationClip hm_appear2;
        private AnimationClip hm_finisher_appear1;

        private Material crossMaterial;
        private Material skullMaterial;

        private void Awake()
        {
            hitAudio = HitmarkersMain.hitAudio;
            hitFinisherAudio = HitmarkersMain.hitFinisherAudio;

            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = true;
            this.source = source;

            GameObject plane = Instantiate(HitmarkersMain.resources.LoadAsset("BaseHitmarker").Cast<GameObject>());

            hm_appear1 = HitmarkersMain.resources.LoadAsset("hm_appear1").Cast<AnimationClip>();
            hm_appear2 = HitmarkersMain.resources.LoadAsset("hm_appear2").Cast<AnimationClip>();
            hm_finisher_appear1 = HitmarkersMain.resources.LoadAsset("hm_finisher_appear1").Cast<AnimationClip>();

            plane.transform.SetParent(transform);

            animator = plane.transform.Find("HM").GetComponent<Animator>();

            deathSkullGO = plane.transform.Find("HM/DeathSkull").gameObject;
            deathSkullGO.SetActive(false);

            crossMaterial = plane.transform.Find("HM").GetComponent<MeshRenderer>().material;
            skullMaterial = plane.transform.Find("HM/DeathSkull").GetComponent<MeshRenderer>().material;

            crossTexture = Utilities.Utilities.LoadFromFile(Utilities.Utilities.textureDir + "cross.png");
            skullTexture = Utilities.Utilities.LoadFromFile(Utilities.Utilities.textureDir + "finisher.png");

            crossMaterial.mainTexture = crossTexture;
            skullMaterial.mainTexture = skullTexture;

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            float distanceFromShot = HitmarkerManager._instance.distanceFromShot;
            float distanceScale = HitmarkerManager._instance.hitmarkerDistanceScale;
            float distanceUntilScale = HitmarkerManager._instance.hitmarkerDistanceUntilScale;
            float hitmarkerAudio = HitmarkerManager._instance.hitmarkerAudio;
            float animationSpeed = HitmarkerManager._instance.animationSpeed;

            Vector3 baseScale = Vector3.one * HitmarkerManager._instance.hitmarkerScale;
            float newDistanceScale = distanceScale * distanceFromShot;

            transform.localScale = distanceFromShot >= distanceUntilScale ? baseScale * newDistanceScale : baseScale;

            AudioClip hitAudioClip = Audio.HitmarkerAudio.hitAudio[Random.Range(0, hitAudio.Count)];
            AudioClip hitFinisherClip = Audio.HitmarkerAudio.hitFinisherAudio[Random.Range(0, hitFinisherAudio.Count)];

            source.volume = hitmarkerAudio;

            animator.speed = animationSpeed;

            if (finisherHitmarker)
            {
                Audio.HitmarkerAudio.PlayAtPoint(hitFinisherClip, transform.position);
                animator.Play("hm_finisher_appear1");
                
            }
            else
            {
                deathSkullGO.SetActive(false);
                Audio.HitmarkerAudio.PlayAtPoint(hitAudioClip, transform.position);
                PlayRandomAnim();
            }

            MelonLoader.MelonCoroutines.Start(CoHide());
        }

        public void UseFinisherHitmarker(bool use)
        {
            finisherHitmarker = use;
        }

        private void PlayRandomAnim()
        {
            int rand = Random.Range(0, 2);

            if(rand == 0)
            {
                animator.Play("hm_appear1");
            }
            else
            {
                animator.Play("hm_appear2");
            }
        }

        private void Update()
        {
            if (finisherHitmarker)
            {
                deathSkullGO.SetActive(HitmarkerManager._instance.useDeathSkull);
            }

            transform.LookAt(HitmarkerManager.GetPlayerHead());
        }

        private IEnumerator CoHide()
        {
            yield return new WaitForSeconds(3f);
            gameObject.SetActive(false);
        }
    }
}
