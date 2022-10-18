using UnityEngine;

using NEP.Hitmarkers.Data;

namespace NEP.Hitmarkers
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class Hitmarker : MonoBehaviour
    {
        public Hitmarker(System.IntPtr ptr) : base(ptr) { }

        public AudioClip[] HitAudio
        {
            get => _hitAudio;
        }

        public AudioClip[] FinisherAudio
        {
            get => _finisherAudio;
        }

        public bool IsFinisher
        {
            get => _isFinisher;
            set => _isFinisher = value;
        }

        private bool _isFinisher;

        private AudioClip[] _hitAudio;
        private AudioClip[] _finisherAudio;

        private Animator _markerAnimator;
        private Animator _finisherAnimator;

        private GameObject _markerObject;
        private GameObject _finisherObject;

        private void Awake()
        {
            _hitAudio = DataManager.HitClips;
            _finisherAudio = DataManager.HitClips;
        }

        private void Start()
        {
            _markerObject = transform.Find("Marker").gameObject;
            _finisherObject = transform.Find("Finisher").gameObject;

            _markerAnimator = _markerObject.GetComponent<Animator>();
            _finisherAnimator = _finisherObject.GetComponent<Animator>();

            _markerObject.SetActive(false);
            _finisherObject.SetActive(false);
        }

        private void OnEnable()
        {
            PlayAnimation();
            PlayAudio();
        }

        private void PlayAnimation()
        {
            var animator = !_isFinisher ? _markerAnimator : _finisherAnimator;
            int rand = Random.Range(0, 2);

            animator.Play($"appear{rand}");
        }

        private void PlayAudio()
        {
            var selectedList = !_isFinisher ? _hitAudio : _finisherAudio;
            AudioClip clip = selectedList[Random.Range(0, selectedList.Length)];

            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
    }
}
