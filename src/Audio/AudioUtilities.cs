using System.IO;
using System.Collections.Generic;

using UnityEngine;

using AudioImportLib;

namespace NEP.Hitmarkers.Audio
{
    public static class AudioUtilities
    {
		public static string hitmarkerDir = MelonLoader.MelonUtils.UserDataDirectory + "/Hitmarkers/Audio/Hitmarkers";
		public static string hitmarkerFinisherDir = MelonLoader.MelonUtils.UserDataDirectory + "/Hitmarkers/Audio/Finishers";

		public static void Intitialize()
        {
			GenerateFolders();

            HitmarkersMain.hitAudio = GetClips(hitmarkerDir);
			HitmarkersMain.hitFinisherAudio = GetClips(hitmarkerFinisherDir);
        }

		private static void GenerateFolders()
        {
            if (!Directory.Exists(hitmarkerDir))
            {
				Directory.CreateDirectory(hitmarkerDir);
            }

			if (!Directory.Exists(hitmarkerFinisherDir))
			{
				Directory.CreateDirectory(hitmarkerFinisherDir);
			}
		}

        // From Dynamic Footsteps - Maranara
        public static List<AudioClip> GetClips(string folder)
        {
            List<AudioClip> clips = new List<AudioClip>();

            string[] files = Directory.GetFiles(folder);

            foreach(string text in files)
            {
				AudioClip clip = API.LoadAudioClip(text, true);

				if(clip == null)
                {
					MelonLoader.MelonLogger.Error("Failed to read " + Path.GetFileName(text));
				}
                else
                {
					clips.Add(clip);
                }
            }

			return clips;
        }
    }
}
