using System;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundManagerSO soundManagerSO;
    [SerializeField] private GameDataSO gameData;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource diegeticSource;

    private AudioClip pausedMusicClip;
    private float pausedMusicTime;

    private void Start()
    {
        SetMusicVolume(gameData.musicVolume);
        SetSFXVolume(gameData.sfxVolume);
    }

    private void OnEnable()
    {
        soundManagerSO.OnPlayMusic += HandlePlayMusic;
        soundManagerSO.OnPlaySFX += HandlePlaySFX;
        soundManagerSO.OnPlayDiegeticMusic += HandleDiegeticMusic;
    }

    private void OnDisable()
    {
        soundManagerSO.OnPlayMusic -= HandlePlayMusic;
        soundManagerSO.OnPlaySFX -= HandlePlaySFX;
        soundManagerSO.OnPlayDiegeticMusic -= HandleDiegeticMusic;
    }

    private void HandlePlayMusic(AudioCueSO cue, string clipId, bool loop)
    {
        musicSource.clip = cue.GetClipById(clipId);
        musicSource.loop = loop;
        musicSource.Play();
    }

    private void HandlePlaySFX(AudioCueSO cue, string clipId, float volume)
    {
        sfxSource.volume = volume;
        sfxSource.PlayOneShot(cue.GetClipById(clipId));
    }

    private void HandleDiegeticMusic(AudioCueSO cue, float duration, string clipId)
    {
        if (musicSource.isPlaying)
        {
            pausedMusicClip = musicSource.clip;
            pausedMusicTime = musicSource.time;
            musicSource.Pause();
        }

        diegeticSource.clip = cue.GetClipById(clipId);
        diegeticSource.Play();

        StartCoroutine(ResumeMainMusicAfterDelay(duration));
    }

    private System.Collections.IEnumerator ResumeMainMusicAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!pausedMusicClip) yield break;
        musicSource.clip = pausedMusicClip;
        musicSource.time = pausedMusicTime;
        diegeticSource.Stop();
        musicSource.Play();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        diegeticSource.volume = volume;
        gameData.musicVolume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        gameData.sfxVolume = volume;
    }
}