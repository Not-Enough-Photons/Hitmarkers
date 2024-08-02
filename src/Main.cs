using MelonLoader;

using BoneLib.BoneMenu;

using Il2CppSLZ.Marrow.Utilities;

using UnityEngine;
using NEP.Hitmarkers.Data;

namespace NEP.Hitmarkers
{
    public static class BuildInfo
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
        public static MelonLogger Logger = new MelonLogger();

        internal const string EmbeddedModule = "NEP.Hitmarkers.Resources.HitmarkersFusionModule.dll";

        public override void OnInitializeMelon()
        {
            Logger = new MelonLogger();
            // var moduleData = Data.DataManager.Internal_LoadFromAssembly(System.Reflection.Assembly.GetExecutingAssembly(), EmbeddedModule);
            // System.Reflection.Assembly.Load(moduleData);

            DataManager.Initialize();
            Options.Initialize();

            SetupBoneMenu();

            MarrowGame.RegisterOnReadyAction(new System.Action(() => OnMarrowGameStart()));
        }

        public void OnMarrowGameStart()
        {
            new GameObject("Hitmarker Manager").AddComponent<HitmarkerManager>();
            HitDirector.Initialize();
        }

        private void SetupBoneMenu()
        {
            Page nepPage = Page.Root.CreatePage("Not Enough Photons", Color.white, maxElements: 8, createLink: true);
            Page hitmarkersPage = nepPage.CreatePage("Hitmarkers", Color.white, maxElements: 0, createLink: true);
            Page customMarkersPage = hitmarkersPage.CreatePage("Skins", Color.white, maxElements: 8, createLink: true);

            hitmarkersPage.CreateBool(
                "Enable Hitmarkers", 
                Color.white, 
                Options.EnableHitmarkers, 
                (value) => Options.SetEnableHitmarkers(value));

            hitmarkersPage.CreateFloat(
                "Hitmarker SFX", 
                Color.white,
                10f,
                Options.HitmarkerSFX, 0f, 100f, 
                (value) => Options.SetHitmarkerVolume(value));

            hitmarkersPage.CreateFloat(
                "Hitmarker Pitch", 
                Color.white, 
                0.25f, 
                Options.HitmarkerPitch, 0f, 2f, 
                (value) => Options.SetHitmarkerPitch(value));

            var favMarkerPage = hitmarkersPage.CreatePage(
                "Set Default Marker",
                Color.white,
                maxElements: 8);

            for (int i = 0; i < DataManager.Skins.Length; i++)
            {
                int index = i;
                customMarkersPage.CreateFunction(DataManager.Skins[index].SkinName, Color.white, () =>
                {
                    HitmarkerManager.Instance.SetMarkerSkin(DataManager.Skins[index]);
                });
            }

            for (int i = 0; i < DataManager.Skins.Length; i++)
            {
                int index = i;
                favMarkerPage.CreateFunction(DataManager.Skins[index].SkinName, Color.white, () =>
                {
                    Options.SetDefaultSkin(DataManager.Skins[index].SkinName);
                });
            }
        }
    }
}