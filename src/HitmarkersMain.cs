using MelonLoader;

using ModThatIsNotMod.BoneMenu;

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

            resources = AssetBundle.LoadFromFile(MelonUtils.UserDataDirectory + "/Hitmarkers/hm_resources.pack");

            MenuCategory menu = MenuManager.CreateCategory("Hitmarkers", Color.white);
            MenuCategory sub_Visuals = menu.CreateSubCategory("Visuals", Color.white);
            MenuCategory sub_Audio = menu.CreateSubCategory("Audio", Color.white);

            menu.CreateBoolElement("Enable Hitmarkers", Color.white, true, (enabled) => enableMod = enabled);

            sub_Visuals.CreateFloatElement("Hitmarker Scale", Color.white, 1f, (num) => HitmarkerManager._instance.hitmarkerScale = num, 0.25f, 0.25f, 2f, true);
            sub_Visuals.CreateFloatElement("Distance Scale", Color.white, 0.15f, (num) => HitmarkerManager._instance.hitmarkerDistanceScale = num, 0.05f, 0.05f, 1f, true);
            sub_Visuals.CreateFloatElement("Distance Until Scale", Color.white, 5, (num) => HitmarkerManager._instance.hitmarkerDistanceUntilScale = num, 1, 1, float.PositiveInfinity, true);
            sub_Visuals.CreateFloatElement("Animation Speed", Color.white, 1f, (num) => HitmarkerManager._instance.animationSpeed = num, 0.25f, 0.25f, 2f, true);

            sub_Audio.CreateFloatElement("Volume", Color.white, 1f, (num) => HitmarkerManager._instance.hitmarkerAudio = num, 0.25f, 0f, 3f, true);

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