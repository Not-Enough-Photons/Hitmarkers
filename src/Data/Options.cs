using MelonLoader;

using BoneLib;

namespace NEP.Hitmarkers.Data
{
    public static class Options
    {
        public static string FavoriteSkin { get; private set; }
        public static bool EnableHitmarkers { get; private set; }
        public static float HitmarkerSFX { get; private set; }
        public static float HitmarkerPitch { get; private set; }

        private static MelonPreferences_Category _category;

        public static ModPref<string> Pref_DefaultSkin => _favoritedSkin;
        public static ModPref<bool> Pref_EnableHitmarkers => _enableHitmarkers;
        public static ModPref<float> Pref_Volume => _hitmarkerVolume;
        public static ModPref<float> Pref_Pitch => _hitmarkerPitch;

        private static ModPref<string> _favoritedSkin;
        private static ModPref<bool> _enableHitmarkers;
        private static ModPref<float> _hitmarkerVolume;
        private static ModPref<float> _hitmarkerPitch;

        public static void Initialize()
        {
            _category = MelonPreferences.CreateCategory("Hitmarkers");

            _favoritedSkin = new ModPref<string>(
                _category,
                "Favorite Skin",
                "Default",
                "Favorite",
                "The favorited skin to use in Hitmarkers.");

            _enableHitmarkers = new ModPref<bool>(
                _category,
                "Enable Hitmarkers",
                true,
                "Enable Hitmarkers",
                "Shows or hides hitmarkers.");

            _hitmarkerVolume = new ModPref<float>(
                _category,
                "Marker Volume",
                100f,
                "Marker Volume",
                "Loudness value for when hits/finishers are made.");

            _hitmarkerPitch = new ModPref<float>(
                _category,
                "Marker Pitch",
                1f,
                "Marker Pitch",
                "Pitch value that controls how high or low pitched the markers are.");

            FavoriteSkin = _favoritedSkin;
            EnableHitmarkers = _enableHitmarkers;
            HitmarkerSFX = _hitmarkerVolume;
            HitmarkerPitch = _hitmarkerPitch;

            MelonPreferences.Save();
        }

        public static void SetDefaultSkin(string skin)
        {
            _favoritedSkin.entry.Value = skin;
            FavoriteSkin = _favoritedSkin;
            MelonPreferences.Save();
        }

        public static void SetEnableHitmarkers(bool enable)
        {
            _enableHitmarkers.entry.Value = enable;
            EnableHitmarkers = _enableHitmarkers;
            MelonPreferences.Save();
        }

        public static void SetHitmarkerVolume(float volume)
        {
            _hitmarkerVolume.entry.Value = volume;
            HitmarkerSFX = _hitmarkerVolume;
            MelonPreferences.Save();
        }

        public static void SetHitmarkerPitch(float pitch)
        {
            _hitmarkerPitch.entry.Value = pitch;
            HitmarkerPitch = _hitmarkerPitch;
            MelonPreferences.Save();
        }
    }
}