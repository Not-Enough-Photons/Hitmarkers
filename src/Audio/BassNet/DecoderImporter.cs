using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using UnityEngine;

namespace NEP.Hitmarkers.Audio
{
    public abstract class DecoderImporter : AudioImporter
    {
        private AudioInfo info;

        private int bufferSize;
        private float[] buffer;

        private int index;

        private bool abort;

        public override void Abort()
        {
            if (abort)
                return;

            abort = true;

            if (!isInitialized)
                UnityEngine.Object.Destroy(audioClip);
        }

        protected override void Import()
        {
            bufferSize = 2048 * 128;
            buffer = new float[bufferSize];

            isDone = false;
            isInitialized = false;
            abort = false;
            index = 0;
            progress = 0;

            DoImport();
        }

        private void DoImport()
        {
            Initialize();

            if (isError)
                return;

            info = GetInfo();

            CreateClip();
            Decode();
            Cleanup();

            progress = 1;
            isDone = true;
        }

        private void Decode()
        {
            while (index < info.lengthSamples)
            {
                int read = GetSamples(buffer, 0, bufferSize);

                if (read == 0)
                    break;

                if (abort)
                    break;

                if (index + bufferSize >= info.lengthSamples)
                    Array.Resize(ref buffer, info.lengthSamples - index);

                SetData();

                index += read;

                progress = (float)index / info.lengthSamples;
            }
        }

        private void CreateClip()
        {
            string name = Path.GetFileNameWithoutExtension(uri.LocalPath);
            audioClip = AudioClip.Create(name, info.lengthSamples / info.channels, info.channels, info.sampleRate, false);
        }

        private void SetData()
        {
            if (audioClip == null)
            {
                Abort();
                return;
            }

            audioClip.SetData(buffer, index / info.channels);

            if (!isInitialized)
            {
                isInitialized = true;
                OnLoaded();
            }
        }

        protected void OnError(string error)
        {
            this.error = error;
            isError = true;

            progress = 1;
        }

        protected abstract void Initialize();

        protected abstract void Cleanup();

        protected abstract int GetSamples(float[] buffer, int offset, int count);

        protected abstract AudioInfo GetInfo();

        protected class AudioInfo
        {
            public int lengthSamples { get; private set; }
            public int sampleRate { get; private set; }
            public int channels { get; private set; }

            public AudioInfo(int lengthSamples, int sampleRate, int channels)
            {
                this.lengthSamples = lengthSamples;
                this.sampleRate = sampleRate;
                this.channels = channels;
            }
        }
    }
}