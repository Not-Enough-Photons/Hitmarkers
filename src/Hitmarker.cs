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

        private Material _markerMaterial;
        private Material _finisherMaterial;
        private Material _finisherSkullMaterial;

        private AudioSource _source;

        private float _timerHide = 0f;
        private float _timeUntilHide = 1f;

        private void Awake()
        {
            _markerObject = transform.Find("Marker").gameObject;
            _finisherObject = transform.Find("Finisher").gameObject;

            _markerAnimator = _markerObject.GetComponent<Animator>();
            _finisherAnimator = _finisherObject.GetComponent<Animator>();

            _source = transform.Find("Source").GetComponent<AudioSource>();

            _markerObject.SetActive(false);
            _finisherObject.SetActive(false);

            _markerMaterial = _markerObject.GetComponent<MeshRenderer>().material;
            _finisherMaterial = _finisherObject.GetComponent<MeshRenderer>().material;
            _finisherSkullMaterial = _finisherObject.transform.Find("DeathSkull").GetComponent<MeshRenderer>().material;
        }

        private void OnEnable()
        {
            SetTextures();
            SetAudio();
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
                transform.LookAt(BoneLib.Player.Head);

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

            if (selectedList == null)
            {
                return;
            }

            BoneLib.Audio.Play2DOneShot(selectedList, BoneLib.Audio.UI, Options.HitmarkerSFX, Options.HitmarkerPitch);
        }

        private void SetAudio()
        {
            MarkerSkin skin = HitmarkerManager.Instance.Skin;
            MarkerSkin favoriteSkin = HitmarkerManager.Instance.FavoriteSkin;
            MarkerSkin defaultSkin = HitmarkerManager.Instance.DefaultSkin;

            if (skin == null)
            {
                _hitAudio = favoriteSkin.HitClips;
                _finisherAudio = favoriteSkin.FinisherClips;
                return;
            }
            else if (skin == null &&  favoriteSkin == null)
            {
                _hitAudio = defaultSkin.HitClips;
                _finisherAudio = defaultSkin.FinisherClips;
                return;
            }
            else if (defaultSkin == null)
            {
                // How did we get here?
                throw new System.NullReferenceException("Default fallback skin is missing!");
            }

            _hitAudio = skin.HitClips != null ? skin.HitClips : favoriteSkin.HitClips;
            _finisherAudio = skin.FinisherClips != null ? skin.FinisherClips : favoriteSkin.FinisherClips;
        }

        private void SetTextures()
        {
            MarkerSkin skin = HitmarkerManager.Instance.Skin;
            MarkerSkin favoriteSkin = HitmarkerManager.Instance.FavoriteSkin;
            MarkerSkin defaultSkin = HitmarkerManager.Instance.DefaultSkin;

            if (skin == null)
            {
                _markerMaterial.mainTexture = favoriteSkin.Marker;
                _finisherMaterial.mainTexture = favoriteSkin.Finisher;
                _finisherSkullMaterial.mainTexture = favoriteSkin.FinisherSkull;
                return;
            }
            else if (skin == null && favoriteSkin == null)
            {
                _markerMaterial.mainTexture = defaultSkin.Marker;
                _finisherMaterial.mainTexture = defaultSkin.Finisher;
                _finisherSkullMaterial.mainTexture = defaultSkin.FinisherSkull;
            }
            else if (defaultSkin == null)
            {
                // How did we get here?
                throw new System.NullReferenceException("Default fallback skin is missing!");
            }

            _markerMaterial.mainTexture = skin.Marker != null ? skin.Marker : favoriteSkin.Marker;
            _finisherMaterial.mainTexture = skin.Finisher != null ? skin.Finisher : favoriteSkin.Finisher;
            _finisherSkullMaterial.mainTexture = skin.FinisherSkull != null ? skin.FinisherSkull : favoriteSkin.FinisherSkull;
        }
    }
}
