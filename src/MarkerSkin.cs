using UnityEngine;

namespace NEP.Hitmarkers
{
    public sealed class MarkerSkin
    {
        public MarkerSkin(string name, Texture2D marker, Texture2D finisher, Texture2D skull, AudioClip[] hitClips, AudioClip[] finisherClips)
        {
            SkinName = name;

            Marker = marker;
            Finisher = finisher;
            FinisherSkull = skull;

            HitClips = hitClips;
            FinisherClips = finisherClips;
        }

        public string SkinName { get; private set; }

        public Texture2D Marker { get; private set; }
        public Texture2D Finisher { get; private set; }
        public Texture2D FinisherSkull { get; private set; }

        public AudioClip[] HitClips { get; private set; }
        public AudioClip[] FinisherClips { get; private set; }
    }
}