using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [Header("UI GameObjects")]
    [SerializeField] private GameObject levelMenuObject;
    [SerializeField] private GameObject inGameElementsObject;
    [SerializeField] private GameObject timeTextObject;
    [SerializeField] private GameObject resumeGameButtonObject;
    [SerializeField] private GameObject nextLevelButtonObject;
    [SerializeField] private GameObject endGameButtonObject;
    [SerializeField] private GameObject helpButtonObject;
    [SerializeField] private GameObject currentMoneyElementObject;
    [SerializeField] private GameObject shelvesBuyButtonObject;
    [SerializeField] private GameObject shelvesSoldOutButtonObject;
    [SerializeField] private GameObject shelvesCapacityPriceElementObject;
    [SerializeField] private GameObject truckCallingBuyButtonObject;
    [SerializeField] private GameObject truckCallingSoldOutButtonObject;
    [SerializeField] private GameObject truckCallingPriceElementObject;
    [SerializeField] private GameObject powerUpDurationBuyButtonObject;
    [SerializeField] private GameObject powerUpDurationSoldOutButtonObject;
    [SerializeField] private GameObject powerUpDurationPriceElementObject;
    [SerializeField] private GameObject extraServiceSlotsBuyButtonObject;
    [SerializeField] private GameObject extraServiceSlotsSoldOutButtonObject;
    [SerializeField] private GameObject extraServiceSlotsPriceElementObject;

    [Header("UI Texts")]
    [SerializeField] private TextMeshProUGUI dayNumberText;
    [SerializeField] private TextMeshProUGUI currentHourText;
    [SerializeField] private TextMeshProUGUI currentMinuteText;
    [SerializeField] private TextMeshProUGUI inGameCurrentMoneyNumberText;
    [SerializeField] private TextMeshProUGUI inGameHappinessNumberText;
    [SerializeField] private TextMeshProUGUI objectiveMoneyNumberText;
    [SerializeField] private TextMeshProUGUI objectiveHappinessNumberText;
    [SerializeField] private TextMeshProUGUI resultsMoneyNumberText;
    [SerializeField] private TextMeshProUGUI resultsHappinessNumberText;
    [SerializeField] private TextMeshProUGUI perkShopMoneyNumberText;
    [SerializeField] private TextMeshProUGUI shelvesLevelNumberText;
    [SerializeField] private TextMeshProUGUI shelvesLevelPriceText;
    [SerializeField] private TextMeshProUGUI truckCallingPriceText;
    [SerializeField] private TextMeshProUGUI powerUpDurationPriceText;
    [SerializeField] private TextMeshProUGUI extraServiceSlotsPriceText;
    [SerializeField] private TextMeshProUGUI finalResultsMoneyNumberText;
    [SerializeField] private TextMeshProUGUI finalResultsHappinessNumberText;

    [Header("UI Images")]
    [SerializeField] private Image currentDayTimeImage;
    [SerializeField] private List<Sprite> dayTimeSprites;
    [SerializeField] private GameObject failureImageObject;
    [SerializeField] private GameObject successImageObject;

    [Header("Scriptable Objects")]
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private PerksSO perksData;

    private int currentDayTimeImageIndex = 0;
    private int minutesCycleIndex = 0;
    private int passedHours = 0;

    void Awake()
    {
        if (!currentDayTimeImage)
        {
            Debug.LogError("Current Day Time Image not assigned.");
            return;
        }

        if (dayTimeSprites != null && dayTimeSprites.Count != 0) return;
        Debug.LogError("Day Time Sprites not assigned or empty.");
        return;
    }
    void OnEnable()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (!gameManager)
        {
            Debug.LogError("GameManager not found in the scene.");
            return;
        }
        gameManager.onGameStart.AddListener(OnGameStart);
        gameManager.onRunEnd.AddListener(OnRunEnd);
        gameManager.onLevelStart.AddListener(OnLevelStart);
        gameManager.onLevelEnd.AddListener(OnLevelEnd);
        gameManager.onEveryQuarterPassed.AddListener(OnEveryQuarterPassed);
        gameManager.onPause.AddListener(OnPause);
        gameManager.onShelvingPerkLVL2Bought.AddListener(OnShelvingPerkLVL2Bought);
        gameManager.onShelvingPerkLVL3Bought.AddListener(OnShelvingPerkLVL3Bought);
        gameManager.onTruckCallingPerkBought.AddListener(OnTruckCallingPerkBought);
        gameManager.onPowerUpDurationPerkBought.AddListener(OnPowerUpDurationPerkBought);
        gameManager.onExtraServiceSlotsPerkBought.AddListener(OnExtraServiceSlotsPerkBought);
        gameManager.onNotEnoughMoneyToBuyPerk.AddListener(OnNotEnoughMoneyToBuyPerk);
        gameManager.onMoneyChanged.AddListener(OnMoneyChanged);
        gameManager.onHappinessChanged.AddListener(OnHappinessChanged);
        gameManager.onObjectivesChanged.AddListener(OnObjectivesChanged);

        shelvesLevelPriceText.text = perksData.perkShelvingLvl2Price.ToString();
        shelvesLevelNumberText.text = "2";
        truckCallingPriceText.text = perksData.perkCallTruckPrice.ToString();
        powerUpDurationPriceText.text = perksData.perkPowerUpDurationPrice.ToString();
        extraServiceSlotsPriceText.text = perksData.perkExtraServiceSlotsPrice.ToString();
    }

    private void OnGameStart()
    {
        dayNumberText.text = (gameData.currentLevelIndex+1).ToString();
    }

    private void OnLevelStart()
    {
        //Debug.Log("Main UI - Level started");
        currentDayTimeImage.sprite = dayTimeSprites[0];
        currentHourText.text = gameData.workdayStartingHour.ToString("00");
        currentMinuteText.text = "00";
    }

    private void OnRunEnd()
    {
        finalResultsHappinessNumberText.text = gameData.happiness.ToString();
        finalResultsMoneyNumberText.text = gameData.currentMoney.ToString();
    }

    private void OnLevelEnd(bool succeeded, bool isLastLevel)
    {
        //Debug.Log("Main UI - Level ended");
        GameObject buttonToShow = isLastLevel ? endGameButtonObject : nextLevelButtonObject;
        buttonToShow.SetActive(true);
        resumeGameButtonObject.SetActive(false);
        helpButtonObject.SetActive(false);
        inGameElementsObject.SetActive(false);
        if (succeeded)
        {
            successImageObject.SetActive(true);
            failureImageObject.SetActive(false);
            buttonToShow.GetComponent<Button>().interactable = true;
        }
        else
        {
            successImageObject.SetActive(false);
            failureImageObject.SetActive(true);
            buttonToShow.GetComponent<Button>().interactable = false;
        }
        levelMenuObject.SetActive(true);
    }

    private void OnPause()
    {
        levelMenuObject.SetActive(true);
        inGameElementsObject.SetActive(false);
    }

    private void OnEveryQuarterPassed()
    {
        //Debug.Log("A QUARTER PASSED");
        minutesCycleIndex++;
        if (minutesCycleIndex >= 4)
        {
            minutesCycleIndex = 0;
            currentMinuteText.text = "00";
            OnEveryHourPassed();
        }
        else
        {
            int minuteValue = 15 * minutesCycleIndex;
            currentMinuteText.text = minuteValue.ToString("00");
        }
    }

    private void OnEveryHourPassed()
    {
        //Debug.Log("An hour passed");
        passedHours++;
        UpdateHourText();
        if (passedHours % 2 == 0)
        {
            OnEveryTwoHoursPassed();
        }
        if (passedHours == 7)
        {
            StartCoroutine(AnimateTimeTextScale(timeTextObject));
        }
    }

    IEnumerator AnimateTimeTextScale(GameObject objectToAnimate)
    {
        Vector3 originalScale = objectToAnimate.transform.localScale;
        Vector3 targetScale = originalScale * 1.5f;
        float duration = 0.4f;
        float elapsed = 0f;

        TextMeshProUGUI[] texts = objectToAnimate.GetComponentsInChildren<TextMeshProUGUI>();
        Color[] originalColors = new Color[texts.Length];
        for (int i = 0; i < texts.Length; i++)
            originalColors[i] = texts[i].color;
        Color targetColor = Color.red;

        // Scale up and color to red
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            objectToAnimate.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            for (int i = 0; i < texts.Length; i++)
                texts[i].color = Color.Lerp(originalColors[i], targetColor, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Scale down and color back to original
        elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            objectToAnimate.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            for (int i = 0; i < texts.Length; i++)
                texts[i].color = Color.Lerp(targetColor, originalColors[i], t);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void OnEveryTwoHoursPassed()
    {
        //Debug.Log("Two hours passed");
        UpdateDayTimeImage();
    }

    private void UpdateHourText()
    {
        string hourText = currentHourText.text;
        int hourValue = int.Parse(hourText);
        hourValue++;
        if (hourValue >= 24)
        {
            hourValue = 0;
        }
        currentHourText.text = hourValue.ToString("00");
    }

    private void UpdateDayTimeImage()
    {
        //Debug.Log("Updated day time image");
        currentDayTimeImageIndex++;
        if (currentDayTimeImageIndex >= dayTimeSprites.Count)
        {
            currentDayTimeImageIndex = 0;
        }
        currentDayTimeImage.sprite = dayTimeSprites[currentDayTimeImageIndex];
    }

    private void OnDisable()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (!gameManager) return;
        gameManager.onGameStart.RemoveListener(OnGameStart);
        gameManager.onRunEnd.RemoveListener(OnRunEnd);
        gameManager.onLevelStart.RemoveListener(OnLevelStart);
        gameManager.onLevelEnd.RemoveListener(OnLevelEnd);
        gameManager.onEveryQuarterPassed.RemoveListener(OnEveryQuarterPassed);
        gameManager.onPause.RemoveListener(OnPause);
        gameManager.onShelvingPerkLVL2Bought.RemoveListener(OnShelvingPerkLVL2Bought);
        gameManager.onShelvingPerkLVL3Bought.RemoveListener(OnShelvingPerkLVL3Bought);
        gameManager.onTruckCallingPerkBought.RemoveListener(OnTruckCallingPerkBought);
        gameManager.onPowerUpDurationPerkBought.RemoveListener(OnPowerUpDurationPerkBought);
        gameManager.onExtraServiceSlotsPerkBought.RemoveListener(OnExtraServiceSlotsPerkBought);
        gameManager.onNotEnoughMoneyToBuyPerk.RemoveListener(OnNotEnoughMoneyToBuyPerk);
        gameManager.onMoneyChanged.RemoveListener(OnMoneyChanged);
        gameManager.onHappinessChanged.RemoveListener(OnHappinessChanged);
        gameManager.onObjectivesChanged.RemoveListener(OnObjectivesChanged);
    }

    private void OnMoneyChanged()
    {
        //Debug.Log("Main UI - Money changed: " + money);
        string newMoneyText = gameData.currentMoney.ToString();
        inGameCurrentMoneyNumberText.text = newMoneyText;
        resultsMoneyNumberText.text = newMoneyText;
        perkShopMoneyNumberText.text = newMoneyText;
    }

    private void OnHappinessChanged()
    {
        //Debug.Log("Main UI - Happiness changed: " + happiness);
        string newHappinessText = gameData.happiness.ToString();
        inGameHappinessNumberText.text = newHappinessText;
        resultsHappinessNumberText.text = newHappinessText;
    }

    private void OnObjectivesChanged(string newMoneyObjectiveText, string newHappinessObjectiveText)
    {
        objectiveMoneyNumberText.text = newMoneyObjectiveText;
        objectiveHappinessNumberText.text = newHappinessObjectiveText;
    }

    private void OnShelvingPerkLVL2Bought()
    {
        shelvesLevelPriceText.text = perksData.perkShelvingLvl3Price.ToString();
        shelvesLevelNumberText.text = "3";
    }

    private void OnShelvingPerkLVL3Bought()
    {
        shelvesLevelNumberText.text = "3";
        shelvesCapacityPriceElementObject.SetActive(false);
        shelvesBuyButtonObject.SetActive(false);
        shelvesSoldOutButtonObject.SetActive(true);
    }

    private void OnTruckCallingPerkBought()
    {
        truckCallingBuyButtonObject.SetActive(false);
        truckCallingSoldOutButtonObject.SetActive(true);
        truckCallingPriceElementObject.SetActive(false);
    }

    private void OnPowerUpDurationPerkBought()
    {
        powerUpDurationBuyButtonObject.SetActive(false);
        powerUpDurationSoldOutButtonObject.SetActive(true);
        powerUpDurationPriceElementObject.SetActive(false);
    }

    private void OnExtraServiceSlotsPerkBought()
    {
        extraServiceSlotsBuyButtonObject.SetActive(false);
        extraServiceSlotsSoldOutButtonObject.SetActive(true);
        extraServiceSlotsPriceElementObject.SetActive(false);
    }

    private void OnNotEnoughMoneyToBuyPerk()
    {
        StartCoroutine(AnimateTimeTextScale(currentMoneyElementObject));        
    }
}
