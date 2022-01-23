using UnityEngine;
using System.IO;

namespace NEP.Hitmarkers.Utilities
{
    public static class Utilities
    {
        public static string textureDir = MelonLoader.MelonUtils.UserDataDirectory + "/Hitmarkers/Textures/";

        public static Texture2D LoadFromFile(string src)
        {
            Texture2D texture = new Texture2D(2, 2);

            if (File.Exists(src))
            {
                byte[] data = File.ReadAllBytes(src);
                ImageConversion.LoadImage(texture, data, false);

                return texture;
            }

            return null;
        }
    }
}
