using UnityEngine;

[System.Serializable]
public class Music 
{
    [Tooltip("Name with no space")]
    public string musicName;
    public AudioClip clip;

    public Music(string musicName, AudioClip clip)
    {
        this.musicName = musicName;
        this.clip = clip;
    }
}
