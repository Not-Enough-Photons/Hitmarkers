﻿using MelonLoader;

using UnityEngine;

using System.Collections.Generic;
using System.Linq;

namespace NEP.Hitmarkers
{
    public static class BuildInfo
    {
        public const string Name = "Hitmarkers"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Simple hitmarkers mod for Boneworks."; // Description for the Mod.  (Set as null if none)
        public const string Author = "Not Enough Photons"; // Author of the Mod.  (MUST BE SET)
        public const string Company = "Not Enough Photons"; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class HitmarkersMain : MelonMod
    {
        public static List<AudioClip> hitAudio { get; set; }
        public static List<AudioClip> hitFinisherAudio { get; set; }

        public static AssetBundle resources;

        public static bool enableMod { get; set; } = true;

        public override void OnApplicationStart()
        {
            hitAudio = new List<AudioClip>();
            hitFinisherAudio = new List<AudioClip>();

            resources = AssetBundle.LoadFromFile(MelonUtils.UserDataDirectory + "Not Enough Photons/Hitmarkers/hm_resources.pack");

            Utilities.HealthPatch.Patch();
            Audio.AudioUtilities.Intitialize();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            new GameObject("Hitmarker Manager").AddComponent<HitmarkerManager>();
        }

        public static Object GetObjectFromResources(string name)
        {
            Object[] objects = resources.LoadAllAssets();

            return objects.FirstOrDefault((asset) => asset.name == name);
        }
    }
}