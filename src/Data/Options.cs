using MelonLoader;

namespace NEP.Hitmarkers.Data
{
    public static class Options
    {
        public static bool EnableHitmarkers { get; private set; }

        public static float HitmarkerSFX { get; private set; }
        public static float HitmarkerPitch { get; private set; }

        private static MelonPreferences_Category category = MelonPreferences.CreateCategory("Hitmarkers");

        private static MelonPreferences_Entry<bool> enableHitmarkers_Entry;
        private static MelonPreferences_Entry<float> hitmarkerVolume_Entry;
        private static MelonPreferences_Entry<float> hitmarkerPitch_Entry;

        public static void Init()
        {
            if (category.GetEntry("Enable Hitmarkers") != null)
            {
                enableHitmarkers_Entry = category.GetEntry<bool>("Enable Hitmarkers");
                EnableHitmarkers = enableHitmarkers_Entry.Value;
            }
            else
            {
                EnableHitmarkers = true;
                enableHitmarkers_Entry = category.CreateEntry("Enable Hitmarkers", EnableHitmarkers);
            }

            if (category.GetEntry("Hitmarker Volume") != null)
            {
                hitmarkerVolume_Entry = category.GetEntry<float>("Hitmarker Volume");
                HitmarkerSFX = hitmarkerVolume_Entry.Value;
            }
            else
            {
                HitmarkerSFX = 100f;
                hitmarkerVolume_Entry = category.CreateEntry("Hitmarker Volume", HitmarkerSFX);
            }

            if (category.GetEntry("Hitmarker Pitch") != null)
            {
                hitmarkerPitch_Entry = category.GetEntry<float>("Hitmarker Pitch");
                HitmarkerPitch = hitmarkerPitch_Entry.Value;
            }
            else
            {
                HitmarkerPitch = 1f;
                hitmarkerPitch_Entry = category.CreateEntry("Hitmarker Pitch", HitmarkerPitch);
            }
        }

        public static void SetEnableHitmarkers(bool value)
        {
            EnableHitmarkers = value;
            enableHitmarkers_Entry.Value = EnableHitmarkers;
        }

        public static void SetHitmarkerSFX(float volume)
        {
            HitmarkerSFX = volume;
            hitmarkerVolume_Entry.Value = HitmarkerSFX;
        }

        public static void SetHitmarkerPitch(float pitch)
        {
            HitmarkerPitch = pitch;
            hitmarkerPitch_Entry.Value = HitmarkerPitch;
        }
    }
}