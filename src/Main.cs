using MelonLoader;

using BoneLib;
using BoneLib.BoneMenu;

using UnityEngine;
using System.Reflection;

namespace NEP.Hitmarkers
{
    public static partial class BuildInfo
    {
        public const string Name = "Hitmarkers"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Simple hitmarkers mod for BONELAB."; // Description for the Mod.  (Set as null if none)
        public const string Author = "Not Enough Photons"; // Author of the Mod.  (MUST BE SET)
        public const string Company = "Not Enough Photons"; // Company that made the Mod.  (Set as null if none)
        public const string Version = "2.8.0"; // Version of the Mod.  (MUST BE SET)
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
            Il2CppSLZ.Marrow.Utilities.MarrowGame.RegisterOnReadyAction(new System.Action(() => OnMarrowGameStart()));
        }

        public void OnMarrowGameStart()
        {
            new UnityEngine.GameObject("Hitmarker Manager").AddComponent<HitmarkerManager>();
            new UnityEngine.GameObject("Hitmarker Audio").AddComponent<Audio.HitmarkerAudio>();
            HitDirector.Initialize();
        }

        private void SetupBoneMenu()
        {
            Page nepPage = Menu.CreatePage("Not Enough Photons");
            Page hitmarkersPage = Menu.CreatePage(parent: nepPage, "Hitmarkers");

            nepPage.CreatePageLink(hitmarkersPage);

            hitmarkersPage.CreateBool(
                "Enable Hitmarkers", 
                Color.white, 
                Data.Options.EnableHitmarkers, 
                (value) => Data.Options.SetEnableHitmarkers(value));

            hitmarkersPage.CreateFloat(
                "Hitmarker SFX", 
                Color.white, 
                Data.Options.HitmarkerSFX, 
                10f, 0f, 100f, 
                (value) => Data.Options.SetHitmarkerSFX(value));

            hitmarkersPage.CreateFloat(
                "Hitmarker Pitch", 
                Color.white, 
                Data.Options.HitmarkerPitch, 
                0.25f, 0f, 2f, 
                (value) => Data.Options.SetHitmarkerPitch(value));
        }
    }
}