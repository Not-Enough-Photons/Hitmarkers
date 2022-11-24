using MelonLoader;

using ModThatIsNotMod.BoneMenu;
using UnityEngine;

namespace NEP.Hitmarkers
{
    public static class BuildInfo
    {
        public const string Name = "Hitmarkers"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Simple hitmarkers mod for BONELAB."; // Description for the Mod.  (Set as null if none)
        public const string Author = "Not Enough Photons"; // Author of the Mod.  (MUST BE SET)
        public const string Company = "Not Enough Photons"; // Company that made the Mod.  (Set as null if none)
        public const string Version = "2.1.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class Main : MelonMod
    {
        public override void OnApplicationStart()
        {
            Data.DataManager.Initialize();
            SetupOptions();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            new UnityEngine.GameObject("Hitmarker Manager").AddComponent<HitmarkerManager>();
            new UnityEngine.GameObject("Hitmarker Audio").AddComponent<Audio.HitmarkerAudio>();
            HitDirector.Initialize();
        }

        private void SetupOptions()
        {
            MenuCategory menu = MenuManager.CreateCategory("Hitmarkers", Color.white);
            MenuCategory sub_Visuals = menu.CreateSubCategory("Visuals", Color.white);
            MenuCategory sub_Audio = menu.CreateSubCategory("Audio", Color.white);

            menu.CreateBoolElement("Enable Hitmarkers", Color.white, true, (enabled) => HitmarkerManager.Instance.EnableHitmarkers = enabled);

            sub_Visuals.CreateFloatElement("Hitmarker Scale", Color.white, 1f, (num) => HitmarkerManager.Instance.HitmarkerScale = num, 0.25f, 0.25f, 2f, true);
            sub_Visuals.CreateFloatElement("Distance Scale", Color.white, 0.15f, (num) => HitmarkerManager.Instance.HitmarkerDistanceScale = num, 0.05f, 0.05f, 1f, true);
            sub_Visuals.CreateFloatElement("Distance Until Scale", Color.white, 5, (num) => HitmarkerManager.Instance.HitmarkerDistanceUntilScale = num, 1, 1, float.PositiveInfinity, true);
            sub_Visuals.CreateFloatElement("Animation Speed", Color.white, 1f, (num) => HitmarkerManager.Instance.HitmarkerAnimationSpeed = num, 0.25f, 0.25f, 2f, true);

            sub_Audio.CreateFloatElement("Volume", Color.white, 1f, (num) => HitmarkerManager.Instance.HitmarkerVolume = num, 0.25f, 0f, 3f, true);
        }
    }
}