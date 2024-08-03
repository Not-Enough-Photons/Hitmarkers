using System.IO;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using MelonLoader.Utils;
using MelonLoader;

using BoneLib;

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

        public static MarkerSkin[] Skins
        {
            get => _skins;
        }

        private static readonly string path_UserData = MelonEnvironment.UserDataDirectory;
        private static readonly string path_Developer = Path.Combine(path_UserData, "Not Enough Photons");
        private static readonly string path_Mod = Path.Combine(path_Developer, "Hitmarkers");

        private static readonly string path_Skins = Path.Combine(path_Mod, "Skins");

        private static AssetBundle _bundle;
        private static Object[] _bundleObjects;
        private static MarkerSkin[] _skins;

        private static List<GameObject> _gameObjects;
        private static List<AnimationClip> _animations;

        internal static bool _initialized;

        public static void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            GenerateFolders();
            LoadBundles();
            LoadObjects();

            _initialized = true;
        }

        public static GameObject GetGameObject(string name)
        {
            return _gameObjects.Find((match) => match.name == name);
        }

        public static MarkerSkin GetMarkerSkin(string name)
        {
            HashSet<MarkerSkin> query = new HashSet<MarkerSkin>(_skins);

            foreach (var skin in query)
            {
                if (skin.SkinName == name)
                {
                    return skin;
                }
            }

            return null;
        }

        public static MarkerSkin[] LoadMarkerSkins()
        {
            string[] directories = Directory.GetDirectories(path_Skins);
            MarkerSkin[] skins = new MarkerSkin[directories.Length];

            for (int i = 0; i < skins.Length; i++)
            {
                skins[i] = CreateMarkerSkin(directories[i]);
            }

            return skins;
        }

        public static MarkerSkin CreateMarkerSkin(string skinPath)
        {
            if (!Directory.Exists(skinPath))
            {
                throw new System.Exception("Path does not exist! " + skinPath);
            }

            string skinName = new DirectoryInfo(skinPath).Name;

            Texture2D marker = LoadTexture(skinPath, "marker.png");
            Texture2D finisher = LoadTexture(skinPath, "finisher_marker.png");
            Texture2D skull = LoadTexture(skinPath, "finisher_feedback.png");

            AudioClip[] hitClips = LoadClips(skinPath, "marker");
            AudioClip[] finisherClips = LoadClips(skinPath, "finisher");

            return new MarkerSkin(skinName, marker, finisher, skull, hitClips, finisherClips);
        }

        private static void LoadBundles()
        {
            string bundleNamespace = "NEP.Hitmarkers.Resources.";
            string bundleName = HelperMethods.IsAndroid() ? "resources_quest.pack" : "resources_pcvr.pack";
            _bundle = HelperMethods.LoadEmbeddedAssetBundle(Assembly.GetExecutingAssembly(), bundleNamespace + bundleName);
            _bundleObjects = _bundle.LoadAllAssets();
        }

        private static void LoadObjects()
        {
            _gameObjects = new List<GameObject>();
            _animations = new List<AnimationClip>();
            GetObjects(_gameObjects);
            GetObjects(_animations);
            _skins = LoadMarkerSkins();
        }

        private static void GenerateFolders()
        {
            Directory.CreateDirectory(path_Mod);
        }

        private static void GetObjects<T>(List<T> list) where T : Object
        {
            foreach (var obj in _bundleObjects)
            {
                var target = obj.TryCast<T>();

                if (target != null)
                {
                    target.hideFlags = HideFlags.DontUnloadUnusedAsset;
                    list.Add(target);
                }
            }
        }

        private static Texture2D LoadTexture(string skinPath, string textureName)
        {
            string assetPath = Path.Combine(skinPath, textureName);
            Texture2D texture = new Texture2D(2, 2);

            if (!File.Exists(assetPath))
            {
                Melon<Main>.Logger.Warning($"{textureName} does not exist in {skinPath}!");
                return null;
            }

            byte[] data = File.ReadAllBytes(assetPath);

            if (!ImageConversion.LoadImage(texture, data))
            {
                Melon<Main>.Logger.Warning($"{textureName} failed to load! The data is likely corrupted or invalid.");
                return null;
            }

            texture.hideFlags = HideFlags.DontUnloadUnusedAsset;
            return texture;
        }

        private static AudioClip[] LoadClips(string skinPath, string searchTerm)
        {
            string sfxPath = Path.Combine(skinPath, "SFX");

            if (!Directory.Exists(sfxPath))
            {
                return null;
            }

            string[] files = Directory.GetFiles(sfxPath);
            List<AudioClip> clips = new List<AudioClip>();

            for (int i = 0; i < files.Length; i++)
            {
                if (new DirectoryInfo(files[i]).Name.StartsWith(searchTerm))
                {
                    clips.Add(AudioImportLib.API.LoadAudioClip(files[i], true));
                }
            }

            return clips.ToArray();
        }
    }
}
