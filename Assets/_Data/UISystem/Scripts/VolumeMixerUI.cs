using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeMixerUI : MonoBehaviour
{
    [Header("Sound Manager and Game Data")]
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private GameDataSO gameData;

    [Header("Music Controls")]
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private Button musicPlusBtn;
    [SerializeField] private Button musicMinusBtn;

    [Header("SFX Controls")]
    [SerializeField] private TextMeshProUGUI sfxVolumeText;
    [SerializeField] private Button sfxPlusBtn;
    [SerializeField] private Button sfxMinusBtn;

    private const int maxVolumeSteps = 10;

    private void Start()
    {
        UpdateMusicUI();
        UpdateSFXUI();

        musicPlusBtn.onClick.AddListener(IncreaseMusicVolume);
        musicMinusBtn.onClick.AddListener(DecreaseMusicVolume);
        sfxPlusBtn.onClick.AddListener(IncreaseSFXVolume);
        sfxMinusBtn.onClick.AddListener(DecreaseSFXVolume);
    }

    private void IncreaseMusicVolume()
    {
        int current = Mathf.RoundToInt(gameData.musicVolume * maxVolumeSteps);
        if (current >= maxVolumeSteps) return;
        
        current++;
        SetMusicVolume(current);
    }

    private void DecreaseMusicVolume()
    {
        int current = Mathf.RoundToInt(gameData.musicVolume * maxVolumeSteps);
        if (current <= 0) return;
        
        current--;
        SetMusicVolume(current);
    }

    private void IncreaseSFXVolume()
    {
        int current = Mathf.RoundToInt(gameData.sfxVolume * maxVolumeSteps);
        if (current >= maxVolumeSteps) return;
        
        current++;
        SetSFXVolume(current);
    }

    private void DecreaseSFXVolume()
    {
        int current = Mathf.RoundToInt(gameData.sfxVolume * maxVolumeSteps);
        if (current <= 0) return;
        
        current--;
        SetSFXVolume(current);
    }

    private void SetMusicVolume(int level)
    {
        float normalized = level / (float)maxVolumeSteps;
        gameData.musicVolume = normalized;
        soundManager.SetMusicVolume(normalized);
        UpdateMusicUI();
    }

    private void SetSFXVolume(int level)
    {
        float normalized = level / (float)maxVolumeSteps;
        gameData.sfxVolume = normalized;
        soundManager.SetSFXVolume(normalized);
        UpdateSFXUI();
    }

    private void UpdateMusicUI()
    {
        int level = Mathf.RoundToInt(gameData.musicVolume * maxVolumeSteps);
        musicVolumeText.text = level.ToString();
    }

    private void UpdateSFXUI()
    {
        int level = Mathf.RoundToInt(gameData.sfxVolume * maxVolumeSteps);
        sfxVolumeText.text = level.ToString();
    }
}
