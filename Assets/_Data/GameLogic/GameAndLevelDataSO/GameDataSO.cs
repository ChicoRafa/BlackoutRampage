using UnityEngine;

[CreateAssetMenu(fileName = "GameDataSO", menuName = "GameDataSO")]
public class GameDataSO : ScriptableObject
{
    [Header("Game Settings")]
    public float levelDurationInMinutes = 8f;
    public int workdayStartingHour = 16;
    public int initialMoneyAmount;
    public int currentMoney;
    public int happiness;
    public int currentLevelIndex = 0;
    
    [Header("Audio Settings")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
}
