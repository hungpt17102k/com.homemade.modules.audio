using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace com.homemade.modules.audio
{
    public class AudioController : MonoBehaviour
    {
        public static AudioController Instance { get; private set; }

        // Key for save load value
        private const string AUDIO_SETTING_SOUND = "Audio_Sound";
        private const string AUDIO_SETTING_MUSIC = "Audio_Music";

        private GameObject _targetGameObject;

        private List<AudioSource> audioSources = new List<AudioSource>();

        private List<AudioSource> activeSounds = new List<AudioSource>();
        private List<AudioSource> activeMusic = new List<AudioSource>();

        private bool _soundState;
        private bool _musicState;

        //---------------------------Unity Functions----------------------------------
        void Awake()
        {
            Instance = this;

            CreateAudioSourceGO();

            LoadAudioSetting();
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
                source.mute = !_soundState;
            }
            else if (type == AudioType.Music)
            {
                source.loop = true;
                source.mute = !_musicState;
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

        // Save and load
        private void SaveAudioSetting()
        {
            AudioUtils.SetBool(AUDIO_SETTING_SOUND, _soundState);
            AudioUtils.SetBool(AUDIO_SETTING_MUSIC, _musicState);
        }

        private void LoadAudioSetting()
        {
            // Load sound
            if (PlayerPrefs.HasKey(AUDIO_SETTING_SOUND))
            {
                _soundState = AudioUtils.GetBool(AUDIO_SETTING_SOUND);
            }
            else
            {
                _soundState = true;
                AudioUtils.SetBool(AUDIO_SETTING_SOUND, _soundState);
            }

            // Load music
            if (PlayerPrefs.HasKey(AUDIO_SETTING_MUSIC))
            {
                _musicState = AudioUtils.GetBool(AUDIO_SETTING_MUSIC);
            }
            else
            {
                _musicState = true;
                AudioUtils.SetBool(AUDIO_SETTING_MUSIC, _musicState);
            }
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

        public void PlaySound(string clip, float volumePercentage = 1.0f, float pitch = 1.0f)
        {
            Sound sound = GetSound(clip);

            if (sound.clip == null)
                Debug.LogError("[AudioController]: Audio clip is null");

            AudioSource source = GetAudioSource();

            SetSourceDefaultSettings(source, AudioType.Sound);

            source.clip = sound.clip;
            source.volume *= volumePercentage;
            source.pitch = pitch;
            source.Play();

            AddSound(source);
        }

        public void PlayMusic(string clip, float volumePercentage = 1.0f)
        {
            Music music = GetMusic(clip);

            if (music.clip == null)
                Debug.LogError("[AudioController]: Audio clip is null");

            AudioSource source = GetAudioSource();

            SetSourceDefaultSettings(source, AudioType.Music);

            source.clip = music.clip;
            source.volume *= volumePercentage;
            source.Play();

            AddMusic(source);
        }

        public AudioCase PlaySmartSound(string clip, float volumePercentage = 1.0f, float pitch = 1.0f)
        {
            Sound sound = GetSound(clip);

            if (sound.clip == null)
                Debug.LogError("[AudioController]: Audio clip is null");

            AudioSource source = GetAudioSource();

            SetSourceDefaultSettings(source, AudioType.Sound);

            source.clip = sound.clip;
            source.volume *= volumePercentage;
            source.pitch = pitch;

            AudioCase audioCase = new AudioCase(sound.clip, source, AudioType.Sound);
            audioCase.Play();

            AddSound(source);

            return audioCase;
        }

        public AudioCase PlaySmartMusic(string clip, float volumePercentage = 1.0f, float pitch = 1.0f)
        {
            Music music = GetMusic(clip);

            if (music.clip == null)
                Debug.LogError("[AudioController]: Audio clip is null");

            AudioSource source = GetAudioSource();

            SetSourceDefaultSettings(source, AudioType.Music);

            source.clip = music.clip;
            source.volume *= volumePercentage;
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

        public void TurnOnSound()
        {
            _soundState = true;
            SaveAudioSetting();
        }

        public void TurnOffSound()
        {
            _soundState = false;
            SaveAudioSetting();

            ReleaseSounds();
        }

        public void TurnOnMusic()
        {
            _musicState = true;
            SaveAudioSetting();
        }

        public void TurnOffMusic()
        {
            _musicState = false;
            SaveAudioSetting();

            ReleaseMusic();
        }

    }
}