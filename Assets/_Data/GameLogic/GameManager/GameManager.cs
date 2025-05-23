using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Header("Sriptable Objects")]
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private List<LevelConfigSO> levelConfigs;
    [SerializeField] private PerksSO perksData;

    [Header("Events")]
    public UnityEvent onGameStart;
    public UnityEvent onLevelStart;
    public UnityEvent onLevelEnd;
    [HideInInspector] public UnityEvent onEveryQuarterPassed;
    [HideInInspector] public UnityEvent onPause;
    [HideInInspector] public UnityEvent onShelvingPerkLVL2Bought;
    [HideInInspector] public UnityEvent onShelvingPerkLVL3Bought;
    [HideInInspector] public UnityEvent onTruckCallingPerkBought;
    [HideInInspector] public UnityEvent onPowerUpDurationPerkBought;
    [HideInInspector] public UnityEvent onExtraServiceSlotsPerkBought;
    [HideInInspector] public UnityEvent onPatienceLevelMultiplierChanged;
    [HideInInspector] public UnityEvent onNotEnoughMoneyToBuyPerk;
    [HideInInspector] public UnityEvent onMoneyChanged;
    [HideInInspector] public UnityEvent onHappinessChanged;
    [HideInInspector] public UnityEvent<string, string> onObjectivesChanged;

    private float levelDuration = 0;
    private float elapsedTime = 0;
    private float quarterDuration;
    private float nextQuarterTime;
    private float passedSeconds = 0;
    private bool levelRunning = false;
    private float patienceLevelMultiplier = 1;

    void Awake()
    {
        levelDuration = gameData.levelDurationInMinutes * 60;
        quarterDuration = levelDuration / 32;
    }
    void Start()
    {
        onGameStart.Invoke();
        Debug.Log("Game Manager - Game started");
        onMoneyChanged.Invoke();
        onHappinessChanged.Invoke();
        UpdateObjective();
        CheckPerks();
    }

    public void StartLevel()
    {
        elapsedTime = 0f;
        passedSeconds = 0;
        nextQuarterTime = quarterDuration;
        levelRunning = true;

        CheckPerks();

        onLevelStart.Invoke();
        Debug.Log("Game Manager - Level started");
    }

    void Update()
    {

        if (levelRunning)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= nextQuarterTime)
            {
                Debug.Log("Game Manager - A quarter passed");
                onEveryQuarterPassed.Invoke();
                nextQuarterTime += quarterDuration;
            }
            if (elapsedTime - passedSeconds >= 1)
            {
                passedSeconds += 1;
                //Debug.Log("Game Manager - Passed seconds: " + passedSeconds);
            }
            if (elapsedTime >= levelDuration)
            {
                EndLevel();
            }
        }
    }
    public void EndLevel()
    {
        levelRunning = false;
        onLevelEnd.Invoke();
        Debug.Log("Game Manager - Level ended");
    }

    public void PauseGame()
    {
        Debug.Log("Game Manager - Game paused");
        levelRunning = false;
        Time.timeScale = 0;
        onPause.Invoke();
    }

    public void ResumeGame()
    {
        levelRunning = true;
        Time.timeScale = 1;
    }

    public void UpdateMoney(int money)
    {
        gameData.money += money;
        onMoneyChanged.Invoke();
    }

    public void UpdateHappiness(int happiness)
    {
        gameData.happiness += happiness;
        onHappinessChanged.Invoke();
    }

    public void UpdateObjective()
    {
        string newMoneyObjective = levelConfigs[gameData.currentLevelIndex].moneyObjective.ToString();
        string newHappinessObjective = levelConfigs[gameData.currentLevelIndex].happinessObjective.ToString();
        onObjectivesChanged.Invoke(newMoneyObjective, newHappinessObjective);
    }

    public void CheckPerks()
    {
        GameObject[] shelvingParents = GameObject.FindGameObjectsWithTag("ShelvingParent");

        foreach (GameObject parent in shelvingParents)
        {
            Transform child1 = parent.transform.GetChild(0);
            Transform child2 = parent.transform.GetChild(1);
            Transform child3 = parent.transform.GetChild(2);

            if (!perksData.perkShelvingLvl2 && !perksData.perkShelvingLvl3)
            {
                child1.gameObject.SetActive(true);
                child2.gameObject.SetActive(false);
                child3.gameObject.SetActive(false);
            }
            else if (perksData.perkShelvingLvl2 && !perksData.perkShelvingLvl3)
            {
                child1.gameObject.SetActive(false);
                child2.gameObject.SetActive(true);
                child3.gameObject.SetActive(false);
                onShelvingPerkLVL2Bought.Invoke();
            }
            else if (perksData.perkShelvingLvl3)
            {
                child1.gameObject.SetActive(false);
                child2.gameObject.SetActive(false);
                child3.gameObject.SetActive(true);
                onShelvingPerkLVL3Bought.Invoke();
            }
        }

        if (perksData.perkCallTruck)
        {
            onTruckCallingPerkBought.Invoke();
        }
        if (perksData.perkPowerUpDuration)
        {
            onPowerUpDurationPerkBought.Invoke();
        }
        if (perksData.perkExtraServiceSlots)
        {
            onExtraServiceSlotsPerkBought.Invoke();
        }
    }

    public void BuyShelvesCapacityPerk()
    {
        if (!perksData.perkShelvingLvl2 && perksData.perkShelvingLvl2Price <= gameData.money)
        {
            UpdateMoney(-perksData.perkShelvingLvl2Price);
            perksData.perkShelvingLvl2 = true;
            onShelvingPerkLVL2Bought.Invoke();
        }
        else if (!perksData.perkShelvingLvl3 && perksData.perkShelvingLvl3Price <= gameData.money)
        {
            UpdateMoney(-perksData.perkShelvingLvl3Price);
            perksData.perkShelvingLvl3 = true;
            onShelvingPerkLVL3Bought.Invoke();
        }
        else
        {
            Debug.Log("Game Manager - Not enough money to buy shelves capacity perk");
            onNotEnoughMoneyToBuyPerk.Invoke();
        }
    }

    public void BuyTruckCallingPerk()
    {
        if (!perksData.perkCallTruck && perksData.perkCallTruckPrice <= gameData.money)
        {
            UpdateMoney(-perksData.perkCallTruckPrice);
            perksData.perkCallTruck = true;
            onTruckCallingPerkBought.Invoke();
        }
        else
        {
            Debug.Log("Game Manager - Not enough money to buy truck calling perk");
            onNotEnoughMoneyToBuyPerk.Invoke();
        }
    }

    public void BuyPowerUpDurationPerk()
    {
        if (!perksData.perkPowerUpDuration && perksData.perkPowerUpDurationPrice <= gameData.money)
        {
            UpdateMoney(-perksData.perkPowerUpDurationPrice);
            perksData.perkPowerUpDuration = true;
            onPowerUpDurationPerkBought.Invoke();
        }
        else
        {
            Debug.Log("Game Manager - Not enough money to buy power up duration perk");
            onNotEnoughMoneyToBuyPerk.Invoke();
        }
    }

    public void BuyExtraServiceSlotsPerk()
    {
        if (!perksData.perkExtraServiceSlots && perksData.perkExtraServiceSlotsPrice <= gameData.money)
        {
            UpdateMoney(-perksData.perkExtraServiceSlotsPrice);
            perksData.perkExtraServiceSlots = true;
            onExtraServiceSlotsPerkBought.Invoke();
        }
        else
        {
            Debug.Log("Game Manager - Not enough money to buy extra service slots perk");
            onNotEnoughMoneyToBuyPerk.Invoke();
        }
    }

    public void ChangePatienceLevelMultiplier(float newValue)
    {
        patienceLevelMultiplier = newValue;
        onPatienceLevelMultiplierChanged?.Invoke();
    }
    public void RestorePatienceLevelMultiplier()
    {
        patienceLevelMultiplier = levelConfigs[gameData.currentLevelIndex].levelBasePatienceMultiplier;
        onPatienceLevelMultiplierChanged?.Invoke();
    }

    public float GetPatienceLevelMultiplier() => patienceLevelMultiplier;
}