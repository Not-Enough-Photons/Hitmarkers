using AudioImportLib;

using UnityEngine;

using System.IO;
using System.Collections.Generic;

namespace NEP.Hitmarkers.Data
{
    public static class DataManager
    {
        public static AssetBundle Bundle
        {
            get => _bundle;
        }

        public static Object[] BundleObjects
        {
            get => _bundleObjects;
        }

        public static GameObject[] GameObjects
        {
            get => _gameObjects.ToArray();
        }

        public static AnimationClip[] Animations
        {
            get => _animations.ToArray();
        }

        public static AudioClip[] HitClips
        {
            get => _clipHitmarkers.ToArray();
        }

        public static AudioClip[] FinisherClips
        {
            get => _clipFinishers.ToArray();
        }

        static readonly string path_UserData = MelonLoader.MelonUtils.UserDataDirectory;
        static readonly string path_Developer = path_UserData + "/Not Enough Photons";
        static readonly string path_Mod = path_Developer + "/Hitmarkers";

        static readonly string path_Resources = path_Mod + "/hm_resources.pack";
        static readonly string path_Audio = path_Mod + "/Audio";

        static readonly string subpath_Hit_Audio = path_Audio + "/Hitmarkers";
        static readonly string subpath_Finisher_Audio = path_Audio + "/Finishers";

        static AssetBundle _bundle;
        static Object[] _bundleObjects;

        static List<GameObject> _gameObjects;
        static List<AnimationClip> _animations;
        static List<AudioClip> _clipHitmarkers;
        static List<AudioClip> _clipFinishers;

        public static void Initialize()
        {
            GenerateFolders();

            _gameObjects = new List<GameObject>();
            _animations = new List<AnimationClip>();
            _clipHitmarkers = new List<AudioClip>();
            _clipFinishers = new List<AudioClip>();

            _bundle = AssetBundle.LoadFromFile(path_Resources);
            _bundleObjects = _bundle.LoadAllAssets();

            GetGameObjects();
            GetAnimations();
            GetAudio();
        }

        public static GameObject GetGameObject(string name)
        {
            return _gameObjects.Find((match) => match.name == name);
        }

        static void GenerateFolders()
        {
            Directory.CreateDirectory(path_Mod);
            Directory.CreateDirectory(path_Audio);

            Directory.CreateDirectory(subpath_Hit_Audio);
            Directory.CreateDirectory(subpath_Finisher_Audio);
        }

        static void GetGameObjects()
        {
            foreach (var obj in _bundleObjects)
            {
                var go = obj.TryCast<GameObject>();

                if (go != null)
                {
                    go.hideFlags = HideFlags.DontUnloadUnusedAsset;
                    _gameObjects.Add(go);
                }
            }
        }

        static void GetAnimations()
        {
            foreach (var obj in _bundleObjects)
            {
                var go = obj.TryCast<AnimationClip>();

                if (go != null)
                {
                    go.hideFlags = HideFlags.DontUnloadUnusedAsset;
                    _animations.Add(go);
                }
            }
        }

        static void GetAudio()
        {
            foreach(var file in Directory.GetFiles(subpath_Hit_Audio))
            {
                _clipHitmarkers.Add(API.LoadAudioClip(file, true));
            }

            foreach(var file in Directory.GetFiles(subpath_Finisher_Audio))
            {
                _clipFinishers.Add(API.LoadAudioClip(file, true));
            }
        }
    }
}
