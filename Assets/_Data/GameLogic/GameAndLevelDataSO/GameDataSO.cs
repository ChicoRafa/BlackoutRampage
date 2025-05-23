using UnityEngine;

[CreateAssetMenu(fileName = "GameDataSO", menuName = "GameDataSO")]
public class GameDataSO : ScriptableObject
{
    public int workdayStartingHour = 16;
    public int money;
    public int happiness;
    public int currentLevelIndex = 0;
}
