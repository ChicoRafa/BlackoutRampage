using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioCue", menuName = "Audio/Audio Cue")]
public class AudioCueSO : ScriptableObject
{
    [SerializeField]
    private List<TaggedAudioClip> clips = new();

    public AudioClip GetRandomClip()
    {
        return clips.Count == 0 ? null : clips[UnityEngine.Random.Range(0, clips.Count)].clip;
    }

    public AudioClip GetClipById(string id)
    {
        foreach (var clip in clips.Where(clip => clip.id.Equals(id, StringComparison.OrdinalIgnoreCase)))
        {
            return clip.clip;
        }

        Debug.LogWarning($"AudioClip with id '{id}' not found.");
        return null;
    }

    public List<AudioClip> GetClipsByTag(AudioTag tag)
    {
        return (from clip in clips where clip.tag == tag select clip.clip).ToList();
    }

    public AudioClip GetRandomClipByTag(AudioTag tag)
    {
        var taggedClips = GetClipsByTag(tag);
        return taggedClips.Count == 0 ? null : taggedClips[UnityEngine.Random.Range(0, taggedClips.Count)];
    }
}