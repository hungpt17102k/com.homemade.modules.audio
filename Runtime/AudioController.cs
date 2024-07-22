using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using com.homemade.pattern.singleton;

namespace com.homemade.modules.audio
{
    public class AudioController : MonoSingleton<AudioController>
    {
        private GameObject _targetGameObject;

        private List<AudioSource> audioSources = new List<AudioSource>();

        private List<AudioSource> activeSounds = new List<AudioSource>();
        private List<AudioSource> activeMusic = new List<AudioSource>();

        private bool soundState = true;
        private bool musicState = true;

        private float soundVolume = 1.0f;
        private float musicVolume = 1.0f;

        //---------------------------Unity Functions----------------------------------
        void Awake()
        {
            CreateAudioSourceGO();
        }

        //-----------------------------Audio Controller Functions--------------------------------

        //====================Private Function===================
        private void CreateAudioSourceGO()
        {
            _targetGameObject = new GameObject("Audio Source Holder");
            _targetGameObject.transform.SetParent(this.transform);
        }

        private AudioSource GetAudioSource()
        {
            int sourcesAmount = audioSources.Count;
            for (int i = 0; i < sourcesAmount; i++)
            {
                if (!audioSources[i].isPlaying)
                {
                    return audioSources[i];
                }
            }

            AudioSource createdSource = CreateAudioSourceObject();
            audioSources.Add(createdSource);

            return createdSource;
        }

        private AudioSource CreateAudioSourceObject()
        {
            AudioSource audioSource = _targetGameObject.AddComponent<AudioSource>();
            SetSourceDefaultSettings(audioSource);

            return audioSource;
        }

        private void SetSourceDefaultSettings(AudioSource source, AudioType type = AudioType.Sound)
        {
            if (type == AudioType.Sound)
            {
                source.loop = false;
                source.mute = !soundState;
            }
            else if (type == AudioType.Music)
            {
                source.loop = true;
                source.mute = !musicState;
            }

            source.clip = null;

            source.volume = 1f;
            source.pitch = 1.0f;
            source.spatialBlend = 0; // 2D Sound
            source.playOnAwake = false;
            source.outputAudioMixerGroup = null;
        }

        private void AddSound(AudioSource source)
        {
            if (!activeSounds.Contains(source))
            {
                activeSounds.Add(source);
            }
        }

        private void AddMusic(AudioSource source)
        {
            if (!activeMusic.Contains(source))
            {
                activeMusic.Add(source);
            }
        }

        private void StopSound(AudioSource source, float fadeTime = 0)
        {
            int streamID = activeSounds.FindIndex(x => x == source);
            if (streamID != -1)
            {
                if (fadeTime == 0)
                {
                    activeSounds[streamID].Stop();
                    activeSounds[streamID].clip = null;
                    activeSounds.RemoveAt(streamID);
                }
                else
                {
                    StartCoroutine(activeSounds[streamID].FadeVolum(0f, fadeTime, callBack: () =>
                    {
                        activeSounds.Remove(source);
                        source.Stop();
                    }));
                }
            }
        }

        private void StopMusic(AudioSource source, float fadeTime = 0)
        {
            int streamID = activeMusic.FindIndex(x => x == source);
            if (streamID != -1)
            {
                if (fadeTime == 0)
                {
                    activeMusic[streamID].Stop();
                    activeMusic[streamID].clip = null;
                    activeMusic.RemoveAt(streamID);
                }
                else
                {
                    StartCoroutine(activeMusic[streamID].FadeVolum(0f, fadeTime, callBack: () =>
                    {
                        activeMusic.Remove(source);
                        source.Stop();
                    }));
                }
            }
        }

        // Get sound audio
        private Sound GetSound(string soundClip)
        {
            AudioClip clip = Resources.Load<AudioClip>($"Audio/Sound/{soundClip}");
            Sound sound = new Sound(soundClip, clip);

            return sound;
        }

        // Get music audio
        private Music GetMusic(string musicClip)
        {
            AudioClip clip = Resources.Load<AudioClip>($"Audio/Music/{musicClip}");
            Music music = new Music(musicClip, clip);

            return music;
        }

        //====================Public Function===================
        public Coroutine InvokeAudioCoroutine(IEnumerator enumerator)
        {
            return StartCoroutine(enumerator);
        }

        public void StopAudioCoroutine(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }

        public void PlaySound(string clip, float pitch = 1.0f)
        {
            Sound sound = GetSound(clip);

            if (sound.clip == null)
                Debug.LogError("[AudioController]: Audio clip is null");

            AudioSource source = GetAudioSource();

            SetSourceDefaultSettings(source, AudioType.Sound);

            source.clip = sound.clip;
            source.volume = this.soundVolume;
            source.pitch = pitch;
            source.Play();

            AddSound(source);
        }

        public void PlayMusic(string clip)
        {
            Music music = GetMusic(clip);

            if (music.clip == null)
                Debug.LogError("[AudioController]: Audio clip is null");

            AudioSource source = GetAudioSource();

            SetSourceDefaultSettings(source, AudioType.Music);

            source.clip = music.clip;
            source.volume = this.musicVolume;
            source.Play();

            AddMusic(source);
        }

        public AudioCase PlaySmartSound(string clip, float pitch = 1.0f)
        {
            Sound sound = GetSound(clip);

            if (sound.clip == null)
                Debug.LogError("[AudioController]: Audio clip is null");

            AudioSource source = GetAudioSource();

            SetSourceDefaultSettings(source, AudioType.Sound);

            source.clip = sound.clip;
            source.volume = this.soundVolume;
            source.pitch = pitch;

            AudioCase audioCase = new AudioCase(sound.clip, source, AudioType.Sound);
            audioCase.Play();

            AddSound(source);

            return audioCase;
        }

        public AudioCase PlaySmartMusic(string clip, float pitch = 1.0f)
        {
            Music music = GetMusic(clip);

            if (music.clip == null)
                Debug.LogError("[AudioController]: Audio clip is null");

            AudioSource source = GetAudioSource();

            SetSourceDefaultSettings(source, AudioType.Music);

            source.clip = music.clip;
            source.volume = this.musicVolume;
            source.pitch = pitch;

            AudioCase audioCase = new AudioCase(music.clip, source, AudioType.Music);

            audioCase.Play();

            AddMusic(source);

            return audioCase;
        }

        public void PlayRandomMusic()
        {
            List<string> musicList = AudioUtils.GetAllNameFile("Resources/Audio/Music");

            System.Random rand = new System.Random();
            int someRandomNumber = rand.Next(0, musicList.Count());
            string musicClip = musicList.ElementAt(someRandomNumber);

            PlayMusic(musicClip);
        }

        // Releasing all active sounds.
        public void ReleaseSounds()
        {
            int activeStreamsCount = activeSounds.Count - 1;
            for (int i = activeStreamsCount; i >= 0; i--)
            {
                activeSounds[i].Stop();
                activeSounds[i].clip = null;
                activeSounds.RemoveAt(i);
            }
        }

        // Releasing all active music.
        public void ReleaseMusic()
        {
            int activeMusicCount = activeMusic.Count - 1;
            for (int i = activeMusicCount; i >= 0; i--)
            {
                activeMusic[i].Stop();
                activeMusic[i].clip = null;
                activeMusic.RemoveAt(i);
            }
        }

        // Stop specific audio case
        public void StopStream(AudioCase audioCase, float fadeTime = 0)
        {
            if (audioCase.type == AudioType.Sound)
            {
                StopSound(audioCase.source, fadeTime);
            }
            else
            {
                StopMusic(audioCase.source, fadeTime);
            }
        }

        public void TurnOnOffSound(bool state)
        {
            soundState = state;

            foreach(var sound in activeSounds)
            {
                sound.mute = soundState; 
            }
        }

        public void TurnOnOffMusic(bool state)
        {
            musicState = state;
            foreach (var music in activeSounds)
            {
                music.mute = musicState;
            }
        }

        public void ChangeSoundVolume(float volume)
        {
            soundVolume = Mathf.Clamp01(volume);

            foreach(var sound in activeSounds)
            {
                sound.volume = soundVolume;
            }
        }

        public void ChangeMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);

            foreach (var music in activeMusic)
            {
                music.volume = musicVolume;
            }
        }

    }
}