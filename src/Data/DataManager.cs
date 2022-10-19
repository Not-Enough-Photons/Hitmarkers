using UnityEngine;

using AudioImportLib;

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

        public static Texture2D[] Textures
        {
            get => _textures.ToArray();
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

        static readonly string path_Resources = path_Mod + "/resources.pack";
        static readonly string path_Audio = path_Mod + "/Audio";
        static readonly string path_Textures = path_Mod + "/Textures";

        static AssetBundle _bundle;
        static Object[] _bundleObjects;

        static List<GameObject> _gameObjects;
        static List<AnimationClip> _animations;
        static List<AudioClip> _clipHitmarkers;
        static List<AudioClip> _clipFinishers;
        static List<Texture2D> _textures;

        public static void Initialize()
        {
            GenerateFolders();

            _gameObjects = new List<GameObject>();
            _animations = new List<AnimationClip>();
            _textures = new List<Texture2D>();
            _clipHitmarkers = new List<AudioClip>();
            _clipFinishers = new List<AudioClip>();

            _bundle = AssetBundle.LoadFromFile(path_Resources);
            _bundleObjects = _bundle.LoadAllAssets();

            GetGameObjects();
            GetAnimations();
            GetAudio();
            GetTextures();
        }

        public static GameObject GetGameObject(string name)
        {
            return _gameObjects.Find((match) => match.name == name);
        }

        public static Texture2D GetTexture(string name)
        {
            return _textures.Find((match) => match.name == name);
        }

        static void GenerateFolders()
        {
            Directory.CreateDirectory(path_Mod);
            Directory.CreateDirectory(path_Textures);
            Directory.CreateDirectory(path_Audio);
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
            foreach(var file in Directory.GetFiles(path_Audio))
            {
                string newName = file.Substring(path_Audio.Length + 1);
                MelonLoader.Melon<Main>.Logger.Msg($"Loading Clip {newName}...");

                if (newName.StartsWith("marker_"))
                {
                    _clipHitmarkers.Add(API.LoadAudioClip(file));
                }
                else if (newName.StartsWith("finisher_"))
                {
                    _clipFinishers.Add(API.LoadAudioClip(file));
                }

                MelonLoader.Melon<Main>.Logger.Msg($"Successfully Loaded {newName}!");
            }
        }

        static void GetTextures()
        {
            foreach (var file in Directory.GetFiles(path_Textures))
            {
                string newName = file.Substring(path_Textures.Length + 1);
                MelonLoader.Melon<Main>.Logger.Msg($"Loading Texture {newName}...");

                Texture2D texture = new Texture2D(2, 2);

                if (!File.Exists(file))
                {
                    MelonLoader.Melon<Main>.Logger.Warning($"Couldn't load {newName}! Going to use a white square instead.");
                }
                else
                {
                    byte[] data = File.ReadAllBytes(file);
                    ImageConversion.LoadImage(texture, data);
                }

                texture.hideFlags = HideFlags.DontUnloadUnusedAsset;
                texture.name = newName;

                MelonLoader.Melon<Main>.Logger.Msg($"Successfully loaded {texture.name}!");
                _textures.Add(texture);
            }
        }
    }
}
