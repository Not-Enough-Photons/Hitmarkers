using UnityEngine;

using System.IO;
using System.Collections.Generic;
using System.Reflection;
using MelonLoader.Utils;

using BoneLib;
using MelonLoader;

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

        static readonly string path_UserData = MelonEnvironment.UserDataDirectory;
        static readonly string path_Developer = Path.Combine(path_UserData, "Not Enough Photons");
        static readonly string path_Mod = Path.Combine(path_Developer, "Hitmarkers");

        static readonly string path_Skins = Path.Combine(path_Mod, "Skins");

        static AssetBundle _bundle;
        static Object[] _bundleObjects;
        static MarkerSkin[] _skins;

        static List<GameObject> _gameObjects;
        static List<AnimationClip> _animations;

        public static void Initialize()
        {
            GenerateFolders();

            _gameObjects = new List<GameObject>();
            _animations = new List<AnimationClip>();

            string bundleNamespace = "NEP.Hitmarkers.Resources.";
            string bundleName = BoneLib.HelperMethods.IsAndroid() ? "resources_quest.pack" : "resources_pcvr.pack";
            _bundle = HelperMethods.LoadEmbeddedAssetBundle(Assembly.GetExecutingAssembly(), bundleNamespace + bundleName);
            _bundleObjects = _bundle.LoadAllAssets();

            GetObjects(_gameObjects);
            GetObjects(_animations);
            _skins = LoadMarkerSkins();
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

            return new MarkerSkin(skinName, marker, finisher, skull, null, null);
        }

        static void GenerateFolders()
        {
            Directory.CreateDirectory(path_Mod);
        }

        static void GetObjects<T>(List<T> list) where T : Object
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

        static Texture2D LoadTexture(string skinPath, string textureName)
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
    }
}
