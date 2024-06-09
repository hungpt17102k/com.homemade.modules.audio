using System;
using UnityEngine;

namespace com.homemade.modules.audio
{
    [Serializable]
    public class AudioCase
    {
        public AudioSource source;
        public AudioType type;

        public event Action OnAudioEnded;

        public AudioCase(AudioClip clip, AudioSource source, AudioType type)
        {
            this.source = source;
            this.type = type;
            this.source.clip = clip;
        }

        public void Complete()
        {
            OnAudioEnded?.Invoke();
        }

        public void Play()
        {
            source.Play();
        }

        public void Stop()
        {
            source.Stop();
        }

    }

    public enum AudioType
    {
        Music = 0,
        Sound = 1
    }
}