using UnityEngine;

[CreateAssetMenu(fileName = "Perks", menuName = "Perks")]
public class PerksSO : ScriptableObject
{
    public int perkShelvingLvl2Price = 0;
    public bool perkShelvingLvl2 = false;
    public int perkShelvingLvl3Price = 0;
    public bool perkShelvingLvl3 = false;

    public int perkCallTruckPrice = 0;
    public bool perkCallTruck = false;
    
    public int perkExtraServiceSlotsPrice = 0;
    public bool perkExtraServiceSlots = false;
    
    public int perkLongerPowerUpsPrice = 0;
    public bool perkLongerPowerUps = false;
}