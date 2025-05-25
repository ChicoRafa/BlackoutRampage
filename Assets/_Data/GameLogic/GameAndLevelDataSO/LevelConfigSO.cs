using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfigSO", menuName = "LevelConfigSO")]
public class LevelConfigSO : ScriptableObject
{
    public float levelBasePatienceMultiplier = 1.0f;
    public int happinessObjective = 0;
    public int moneyObjective = 0;
}
