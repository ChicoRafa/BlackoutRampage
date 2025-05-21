using UnityEngine;

[CreateAssetMenu(fileName = "Perks", menuName = "Perks")]
public class PerksSO : ScriptableObject
{
    public bool perkShelvingLvl2 = false;
    public bool perkShelvingLvl3 = false;

    public bool perkCallTruck = false;
}