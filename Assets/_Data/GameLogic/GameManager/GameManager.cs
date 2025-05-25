using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Sriptable Objects")]
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private List<LevelConfigSO> levelConfigs;
    [SerializeField] private PerksSO perksData;

    [Header("Events")]
    public UnityEvent onGameStart;
    [HideInInspector] public UnityEvent onRunEnd;
    public UnityEvent onLevelStart;
    public UnityEvent<bool, bool> onLevelEnd;
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

    private void Awake()
    {
        levelDuration = gameData.levelDurationInMinutes * 60;
        quarterDuration = levelDuration / 32;
    }
    private void Start()
    {
        onGameStart.Invoke();
        onMoneyChanged.Invoke();
        onHappinessChanged.Invoke();
        UpdateObjective();
        CheckPerks();
    }

    public void EndRun()
    {
        levelRunning = false;
        onRunEnd.Invoke();
    }

    public void StartLevel()
    {
        elapsedTime = 0f;
        passedSeconds = 0;
        nextQuarterTime = quarterDuration;
        levelRunning = true;

        CheckPerks();

        onLevelStart.Invoke();
    }

    private void Update()
    {
        if (!levelRunning)
            return;

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= nextQuarterTime)
        {
            onEveryQuarterPassed.Invoke();
            nextQuarterTime += quarterDuration;
        }

        if (elapsedTime - passedSeconds >= 1)
            passedSeconds += 1;

        if (elapsedTime >= levelDuration)
            EndLevel();
    }
    public void EndLevel()
    {
        levelRunning = false;
        bool isLastLevel = gameData.currentLevelIndex >= levelConfigs.Count - 1;
        bool succeeded = gameData.currentMoney >= levelConfigs[gameData.currentLevelIndex].moneyObjective &&
                          gameData.happiness >= levelConfigs[gameData.currentLevelIndex].happinessObjective;

        onLevelEnd.Invoke(succeeded, isLastLevel);
    }

    public void PauseGame()
    {
        levelRunning = false;
        Time.timeScale = 0;
        onPause.Invoke();
    }

    public void ResumeGame()
    {
        levelRunning = true;
        Time.timeScale = 1;
    }

    public int GetCurrentMoney()
    {
        return gameData.currentMoney;
    }
    public void UpdateMoney(int money)
    {
        gameData.currentMoney += money;
        onMoneyChanged.Invoke();
    }

    public void UpdateHappiness(int happiness)
    {
        gameData.happiness += happiness;
        if (gameData.happiness < 0) gameData.happiness = 0;
        onHappinessChanged.Invoke();
    }

    public void UpdateObjective()
    {
        if (gameData.currentLevelIndex >= 0 && gameData.currentLevelIndex < levelConfigs.Count)
        {
            string newMoneyObjective = levelConfigs[gameData.currentLevelIndex].moneyObjective.ToString();
            string newHappinessObjective = levelConfigs[gameData.currentLevelIndex].happinessObjective.ToString();
            onObjectivesChanged.Invoke(newMoneyObjective, newHappinessObjective);
        }
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
            onTruckCallingPerkBought.Invoke();

        if (perksData.perkPowerUpDuration)
            onPowerUpDurationPerkBought.Invoke();

        if (perksData.perkExtraServiceSlots)
            onExtraServiceSlotsPerkBought.Invoke();
    }

    public void BuyShelvesCapacityPerk()
    {
        if (!perksData.perkShelvingLvl2 && perksData.perkShelvingLvl2Price <= gameData.currentMoney)
        {
            UpdateMoney(-perksData.perkShelvingLvl2Price);
            perksData.perkShelvingLvl2 = true;
            onShelvingPerkLVL2Bought.Invoke();
        }
        else if (!perksData.perkShelvingLvl3 && perksData.perkShelvingLvl3Price <= gameData.currentMoney)
        {
            UpdateMoney(-perksData.perkShelvingLvl3Price);
            perksData.perkShelvingLvl3 = true;
            onShelvingPerkLVL3Bought.Invoke();
        }
        else
            onNotEnoughMoneyToBuyPerk.Invoke();
    }

    public void BuyTruckCallingPerk()
    {
        if (!perksData.perkCallTruck && perksData.perkCallTruckPrice <= gameData.currentMoney)
        {
            UpdateMoney(-perksData.perkCallTruckPrice);
            perksData.perkCallTruck = true;
            onTruckCallingPerkBought.Invoke();
        }
        else
            onNotEnoughMoneyToBuyPerk.Invoke();
    }

    public void BuyPowerUpDurationPerk()
    {
        if (!perksData.perkPowerUpDuration && perksData.perkPowerUpDurationPrice <= gameData.currentMoney)
        {
            UpdateMoney(-perksData.perkPowerUpDurationPrice);
            perksData.perkPowerUpDuration = true;
            onPowerUpDurationPerkBought.Invoke();
        }
        else
            onNotEnoughMoneyToBuyPerk.Invoke();
    }

    public void BuyExtraServiceSlotsPerk()
    {
        if (!perksData.perkExtraServiceSlots && perksData.perkExtraServiceSlotsPrice <= gameData.currentMoney)
        {
            UpdateMoney(-perksData.perkExtraServiceSlotsPrice);
            perksData.perkExtraServiceSlots = true;
            onExtraServiceSlotsPerkBought.Invoke();
        }
        else
            onNotEnoughMoneyToBuyPerk.Invoke();
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

    public void InitializeGameData()
    {
        gameData.currentMoney = gameData.initialMoneyAmount;
        gameData.happiness = 0;
        gameData.currentLevelIndex = 0;
        perksData.perkShelvingLvl2 = false;
        perksData.perkShelvingLvl3 = false;
        perksData.perkCallTruck = false;
        perksData.perkPowerUpDuration = false;
        perksData.perkExtraServiceSlots = false;
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadNextLevel()
    {
        gameData.currentLevelIndex++;
        if (gameData.currentLevelIndex < levelConfigs.Count)
            SceneManager.LoadScene(gameData.currentLevelIndex+1);
        else
            LoadMainMenu();
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene(1);
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
