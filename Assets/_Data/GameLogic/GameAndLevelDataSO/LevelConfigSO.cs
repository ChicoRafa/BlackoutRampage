using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfigSO", menuName = "LevelConfigSO")]
public class LevelConfigSO : ScriptableObject
{
    public float levelBasePatienceMultiplier = 1.0f;
    public float patienceMultiplierForKarenExplosion = 0.5f;
    public float speedMultiplierForLightingPowerUpTaken = 1.2f;
    public int happinessObjective = 0;
    public int moneyObjective = 0;
}
