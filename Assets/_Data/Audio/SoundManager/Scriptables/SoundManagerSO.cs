using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SoundManager", menuName = "Audio/Sound Manager")]
public class SoundManagerSO : ScriptableObject
{
    [Header("Audio Source")]
    public static AudioSource SoundObject;

    [Header("Audio Actions")]
    public UnityAction<AudioCueSO, string, bool> OnPlayMusic;
    public UnityAction<AudioCueSO, string, float> OnPlaySFX;
    public UnityAction<AudioCueSO, float, string> OnPlayDiegeticMusic;

    public void PlayMusic(AudioCueSO cue, string clipId, bool loop = true) => OnPlayMusic?.Invoke(cue, clipId, loop);
    public void PlaySFX(AudioCueSO cue, string clipId, float volume) => OnPlaySFX?.Invoke(cue, clipId, volume);
    public void PlayDiegeticMusic(AudioCueSO cue, float duration, string clipId) => OnPlayDiegeticMusic?.Invoke(cue, duration, clipId);
}
