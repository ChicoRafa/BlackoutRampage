using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SoundManager", menuName = "Audio/Sound Manager")]
public class SoundManagerSO : ScriptableObject
{
    public static AudioSource SoundObject;

    public UnityAction<AudioCueSO, string, bool> OnPlayMusic;
    public UnityAction<AudioCueSO, string> OnPlaySFX;
    public UnityAction<AudioCueSO, float, string> OnPlayDiegeticMusic;

    public void PlayMusic(AudioCueSO cue, string clipId, bool loop = true) => OnPlayMusic?.Invoke(cue, clipId, loop);
    public void PlaySFX(AudioCueSO cue, string clipId) => OnPlaySFX?.Invoke(cue, clipId);
    public void PlayDiegeticMusic(AudioCueSO cue, float duration, string clipId) => OnPlayDiegeticMusic?.Invoke(cue, duration, clipId);
}