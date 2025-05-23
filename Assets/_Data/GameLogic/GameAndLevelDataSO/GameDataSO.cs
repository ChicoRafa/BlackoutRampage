using UnityEngine;

[CreateAssetMenu(fileName = "GameDataSO", menuName = "GameDataSO")]
public class GameDataSO : ScriptableObject
{
    public float levelDurationInMinutes = 8f;
    public int workdayStartingHour = 16;
    public int money;
    public int happiness;
    public int currentLevelIndex = 0;
}
