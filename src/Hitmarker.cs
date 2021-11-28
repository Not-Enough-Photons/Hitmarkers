﻿using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace NEP.Hitmarkers
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class Hitmarker : MonoBehaviour
    {
        public Hitmarker(System.IntPtr ptr) : base(ptr) { }

        public Animator animator { get; private set; }

        public List<AudioClip> hitAudio { get; private set; }
        public List<AudioClip> hitFinisherAudio { get; private set; }

        private AudioSource _source;

        public bool finisherHitmarker = false;

        private void Awake()
        {
            hitAudio = HitmarkersMain.hitAudio;
            hitFinisherAudio = HitmarkersMain.hitFinisherAudio;

            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = true;
            _source = source;

            GameObject plane = Instantiate(HitmarkersMain.resources.LoadAsset("BaseHitmarker").Cast<GameObject>());

            plane.transform.SetParent(transform);

            animator = plane.transform.Find("HM").GetComponent<Animator>();

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            float distanceFromShot = HitmarkerManager._instance.distanceFromShot;
            float distanceScale = HitmarkerManager._instance.hitmarkerDistanceScale;
            float distanceUntilScale = HitmarkerManager._instance.hitmarkerDistanceUntilScale;

            Vector3 baseScale = Vector3.one * HitmarkerManager._instance.hitmarkerScale;
            float newDistanceScale = distanceScale * distanceFromShot;

            transform.localScale = distanceFromShot >= distanceUntilScale ? baseScale * newDistanceScale : baseScale;

            AudioClip hitAudioClip = hitAudio[Random.Range(0, hitAudio.Count)];
            AudioClip hitFinisherClip = hitFinisherAudio[Random.Range(0, hitFinisherAudio.Count)];

            if (finisherHitmarker)
            {
                _source.PlayOneShot(hitFinisherClip);
                animator.Play("hm_finisher_appear1");
            }
            else
            {
                _source.PlayOneShot(hitAudioClip);
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
            transform.LookAt(HitmarkerManager.GetPlayerHead());
        }

        private IEnumerator CoHide()
        {
            yield return new WaitForSeconds(0.95f);
            gameObject.SetActive(false);
        }
    }
}
