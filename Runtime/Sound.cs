using UnityEngine;

[System.Serializable]
public class Sound
{
    [Tooltip("Name with no space")]
    public string soundName;
    public AudioClip clip;

    public Sound(string soundName, AudioClip clip)
    {
        this.soundName = soundName;
        this.clip = clip;
    }
}
