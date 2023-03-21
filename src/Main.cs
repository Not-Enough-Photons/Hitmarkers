using MelonLoader;

using BoneLib;
using BoneLib.BoneMenu;
using BoneLib.BoneMenu.Elements;

using SLZ.Marrow.Utilities;

using UnityEngine;
using System.Reflection;

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
        internal const string EmbeddedModule = "NEP.Hitmarkers.Resources.HitmarkersFusionModule.dll";

        public override void OnInitializeMelon()
        {
            var moduleData = Data.DataManager.Internal_LoadFromAssembly(Assembly.GetExecutingAssembly(), EmbeddedModule);
            Assembly.Load(moduleData);

            Data.Options.Init();

            SetupBoneMenu();

            Data.DataManager.Initialize();
            MarrowGame.RegisterOnReadyAction(new System.Action(() => OnMarrowGameStart()));
        }

        public void OnMarrowGameStart()
        {
            new UnityEngine.GameObject("Hitmarker Manager").AddComponent<HitmarkerManager>();
            new UnityEngine.GameObject("Hitmarker Audio").AddComponent<Audio.HitmarkerAudio>();
            HitDirector.Initialize();
        }

        private void SetupBoneMenu()
        {
            MenuCategory hitmarkersCategory = null;

            if (MenuManager.GetCategory("Not Enough Photons") != null)
            {
                hitmarkersCategory = MenuManager.GetCategory("Not Enough Photons");
            }
            else
            {
                hitmarkersCategory = MenuManager.CreateCategory("Not Enough Photons", "#08104d");
            }

            var options = hitmarkersCategory.CreateCategory("Hitmarkers", Color.white);

            options.CreateBoolElement(
                "Enable Hitmarkers", 
                Color.white, 
                Data.Options.EnableHitmarkers, 
                (value) => Data.Options.SetEnableHitmarkers(value));

            options.CreateFloatElement(
                "Hitmarker SFX", 
                Color.white, 
                Data.Options.HitmarkerSFX, 
                10f, 0f, 100f, 
                (value) => Data.Options.SetHitmarkerSFX(value));

            options.CreateFloatElement(
                "Hitmarker Pitch", 
                Color.white, 
                Data.Options.HitmarkerPitch, 
                0.25f, 0f, 2f, 
                (value) => Data.Options.SetHitmarkerPitch(value));
        }
    }
}