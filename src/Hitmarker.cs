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

        private AudioSource _source;

        private float _timerHide = 0f;
        private float _timeUntilHide = 1f;

        private void Awake()
        {
            _hitAudio = DataManager.HitClips;
            _finisherAudio = DataManager.FinisherClips;

            _markerObject = transform.Find("Marker").gameObject;
            _finisherObject = transform.Find("Finisher").gameObject;

            _markerAnimator = _markerObject.GetComponent<Animator>();
            _finisherAnimator = _finisherObject.GetComponent<Animator>();

            _source = transform.Find("Source").GetComponent<AudioSource>();

            SetTextures();

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
            if (_isFinisher)
            {
                _finisherObject.SetActive(true);
                _markerObject.SetActive(false);

                int rand = Random.Range(1, 2);
                _finisherAnimator.Play($"finisher_appear_{rand}");
            }
            else
            {
                _finisherObject.SetActive(false);
                _markerObject.SetActive(true);

                int rand = Random.Range(1, 2);
                _markerAnimator.Play($"marker_appear_{rand}");
            }
        }

        private void Update()
        {
            if (gameObject.activeInHierarchy)
            {
                transform.LookAt(BoneLib.Player.playerHead);

                _timerHide += Time.deltaTime;

                if (_timerHide > _timeUntilHide)
                {
                    gameObject.SetActive(false);
                    _timerHide = 0f;
                }
            }
        }

        private void PlayAudio()
        {
            var selectedList = !_isFinisher ? _hitAudio : _finisherAudio;
            AudioClip clip = selectedList[Random.Range(0, selectedList.Length)];

            Audio.HitmarkerAudio.PlayAtPoint(clip, transform.position);
        }

        private void SetTextures()
        {
            Material markerMaterial = _markerObject.GetComponent<MeshRenderer>().material;
            Material finisherMaterial = _finisherObject.GetComponent<MeshRenderer>().material;
            Material finisherSkullMaterial = _finisherObject.transform.Find("DeathSkull").GetComponent<MeshRenderer>().material;

            markerMaterial.mainTexture = DataManager.GetTexture("marker.png");
            finisherMaterial.mainTexture = DataManager.GetTexture("finisher_marker.png");
            finisherSkullMaterial.mainTexture = DataManager.GetTexture("finisher_feedback.png");
        }
    }
}
