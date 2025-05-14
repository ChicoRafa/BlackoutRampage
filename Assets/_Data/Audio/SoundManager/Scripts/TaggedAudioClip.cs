using UnityEngine;

public enum AudioTag
{
    Music,
    SFX,
    Diegetic
}

[System.Serializable]
public class TaggedAudioClip
{
    public string id;
    public AudioTag tag;
    public AudioClip clip;
}
