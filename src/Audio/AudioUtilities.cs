using System.IO;
using System.Collections.Generic;

using UnityEngine;

namespace NEP.Hitmarkers.Audio
{
    public static class AudioUtilities
    {
		public static string hitmarkerDir = MelonLoader.MelonUtils.UserDataDirectory + "/Hitmarkers/Audio/Hitmarkers";
		public static string hitmarkerFinisherDir = MelonLoader.MelonUtils.UserDataDirectory + "/Hitmarkers/Audio/Finishers";

		public static List<string> allowedExtensions = new List<string>() { ".wav", ".mp3", ".ogg" };

		public static void Intitialize()
        {
			GenerateFolders();

			BassImporter reader = new BassImporter();

			HitmarkersMain.hitAudio = GetClips(hitmarkerDir, reader);
			HitmarkersMain.hitFinisherAudio = GetClips(hitmarkerFinisherDir, reader);
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
        public static List<AudioClip> GetClips(string folder, BassImporter importer)
        {
            List<AudioClip> clips = new List<AudioClip>();

            string[] files = Directory.GetFiles(folder);

            foreach(string text in files)
            {
				AudioClip clip = ReadClip(text, importer);

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

        public static AudioClip ReadClip(string file, BassImporter reader)
        {
			bool notExtension = !allowedExtensions.Contains(Path.GetExtension(file));

			AudioClip result;

			if (notExtension)
			{
				result = null;
			}
			else
			{
				reader.Import(file);
				bool notNull = reader.audioClip == null;
				if (notNull)
				{
					MelonLoader.MelonLogger.Error("Failed to import " + Path.GetFileName(file));
					result = null;
				}
				else
				{
					reader.audioClip.hideFlags = HideFlags.DontUnloadUnusedAsset;
					result = reader.audioClip;
				}
			}

			return result;
		}
    }
}
