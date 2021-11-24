using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace NEP.Hitmarkers
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class Hitmarker : MonoBehaviour
    {
        public Hitmarker(System.IntPtr ptr) : base(ptr) { }

        public Animator animator { get; private set; }

        public MeshRenderer renderer { get; private set; }

        public List<AudioClip> hitAudio { get; private set; }
        public List<AudioClip> hitFinisherAudio { get; private set; }

        private List<AnimationClip> hitmarkerAnimations;

        private AudioSource _source;

        public bool finisherHitmarker = false;

        private void Awake()
        {
            hitAudio = HitmarkersMain.hitAudio;
            hitFinisherAudio = HitmarkersMain.hitFinisherAudio;


            hitmarkerAnimations = new List<AnimationClip>()
            {
                HitmarkersMain.resources.LoadAsset("hm_appear1").Cast<AnimationClip>(),
                HitmarkersMain.resources.LoadAsset("hm_appear2").Cast<AnimationClip>()
            };

            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = true;
            _source = source;

            GameObject plane = Instantiate(HitmarkersMain.resources.LoadAsset("BaseHitmarker").Cast<GameObject>());

            plane.transform.SetParent(transform);

            renderer = plane.GetComponent<MeshRenderer>();
            animator = plane.GetComponent<Animator>();

            AudioClip hitAudioClip = hitAudio[Random.Range(0, hitAudio.Count)];
            AudioClip hitFinisherClip = hitFinisherAudio[Random.Range(0, hitFinisherAudio.Count)];

            if (finisherHitmarker)
            {
                _source.clip = hitFinisherClip;
            }
            else
            {
                _source.clip = hitAudioClip;
            }

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            PlayRandomAnim();

            MelonLoader.MelonCoroutines.Start(CoHide());
        }

        private void OnDiable()
        {
            finisherHitmarker = false;
        }

        public void SetClip(AudioClip clip)
        {
            if(clip == null || _source == null) { return; }

            _source.clip = clip;
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
            transform.LookAt(ModThatIsNotMod.Player.GetPlayerHead().transform);
        }

        private IEnumerator CoHide()
        {
            yield return new WaitForSeconds(0.85f);
            gameObject.SetActive(false);
        }
    }
}
