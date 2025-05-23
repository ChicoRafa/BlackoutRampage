using System.Collections;
using System.Collections.Generic;
using _Data.PlayerController.Scripts;
using TMPro;
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
    [SerializeField] private GameObject helpButtonObject;
    [SerializeField] private GameObject shelvesBuyButtonObject;
    [SerializeField] private GameObject shelvesSoldOutButtonObject;
    [SerializeField] private GameObject shelvesCapacityPriceElementObject;
    [SerializeField] private GameObject truckCallingBuyButtonObject;
    [SerializeField] private GameObject truckCallingSoldOutButtonObject;
    [SerializeField] private GameObject truckCallingPriceElementObject;

    [Header("UI Texts")]
    [SerializeField] private TextMeshProUGUI currentHourText;
    [SerializeField] private TextMeshProUGUI currentMinuteText;
    [SerializeField] private TextMeshProUGUI inGameCurrentMoneyNumberText;
    [SerializeField] private TextMeshProUGUI inGameHappinessNumberText;
    [SerializeField] private TextMeshProUGUI resultsMoneyNumberText;
    [SerializeField] private TextMeshProUGUI resultsHappinessNumberText;
    [SerializeField] private TextMeshProUGUI perkShopMoneyNumberText;
    [SerializeField] private TextMeshProUGUI shelvesLevelNumberText;
    [SerializeField] private TextMeshProUGUI shelvesLevelPriceText;
    [SerializeField] private TextMeshProUGUI truckCallingPriceText;

    [Header("UI Images")]
    [SerializeField] private Image currentDayTimeImage;
    [SerializeField] private List<Sprite> dayTimeSprites;
    [SerializeField] private Image failureImage;
    [SerializeField] private Image successImage;

    [Header("Scriptable Objects")]
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private PerksSO perksData;

    private int currentDayTimeImageIndex = 0;
    private int minutesCycleIndex = 0;
    private int passedHours = 0;
    
    void Awake()
    {
        if (currentDayTimeImage == null)
        {
            Debug.LogError("Current Day Time Image not assigned.");
            return;
        }
        if (dayTimeSprites == null || dayTimeSprites.Count == 0)
        {
            Debug.LogError("Day Time Sprites not assigned or empty.");
            return;
        }
    }
    void OnEnable()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
            return;
        }
        gameManager.onLevelStart.AddListener(OnLevelStart);
        gameManager.onLevelEnd.AddListener(OnLevelEnd);
        gameManager.onEveryQuarterPassed.AddListener(OnEveryQuarterPassed);
        gameManager.onPause.AddListener(OnPause);
        gameManager.onShelvingPerkLVL2Bought.AddListener(OnShelvingPerkLVL2Bought);
        gameManager.onShelvingPerkLVL3Bought.AddListener(OnShelvingPerkLVL3Bought);
        gameManager.onTruckCallingPerkBought.AddListener(OnTruckCallingPerkBought);
        gameManager.onMoneyChanged.AddListener(OnMoneyChanged);
        gameManager.onHappinessChanged.AddListener(OnHappinessChanged);

        shelvesLevelPriceText.text = perksData.perkShelvingLvl2Price.ToString();
        shelvesLevelNumberText.text = "2";
        truckCallingPriceText.text = perksData.perkCallTruckPrice.ToString();
    }

    private void OnLevelStart()
    {
        //Debug.Log("Main UI - Level started");
        currentDayTimeImage.sprite = dayTimeSprites[0];
        currentHourText.text = gameData.workdayStartingHour.ToString("00");
        currentMinuteText.text = "00";
    }

    private void OnLevelEnd()
    {
        //Debug.Log("Main UI - Level ended");

        nextLevelButtonObject.SetActive(true);
        resumeGameButtonObject.SetActive(false);
        helpButtonObject.SetActive(false);
        failureImage.gameObject.SetActive(true);
        levelMenuObject.SetActive(true);
        inGameElementsObject.SetActive(false);
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
            StartCoroutine(AnimateTimeTextScale());
        }
    }

    IEnumerator AnimateTimeTextScale()
    {
        Vector3 originalScale = timeTextObject.transform.localScale;
        Vector3 targetScale = originalScale * 1.5f;
        float duration = 0.4f;
        float elapsed = 0f;

        TextMeshProUGUI[] texts = timeTextObject.GetComponentsInChildren<TextMeshProUGUI>();
        Color[] originalColors = new Color[texts.Length];
        for (int i = 0; i < texts.Length; i++)
            originalColors[i] = texts[i].color;
        Color targetColor = Color.red;

        // Scale up and color to red
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            timeTextObject.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
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
            timeTextObject.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
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
        if (gameManager != null)
        {
            gameManager.onLevelStart.RemoveListener(OnLevelStart);
            gameManager.onLevelEnd.RemoveListener(OnLevelEnd);
            gameManager.onEveryQuarterPassed.RemoveListener(OnEveryQuarterPassed);
            gameManager.onPause.RemoveListener(OnPause);
            gameManager.onShelvingPerkLVL2Bought.RemoveListener(OnShelvingPerkLVL2Bought);
            gameManager.onShelvingPerkLVL3Bought.RemoveListener(OnShelvingPerkLVL3Bought);
            gameManager.onTruckCallingPerkBought.RemoveListener(OnTruckCallingPerkBought);
            gameManager.onMoneyChanged.RemoveListener(OnMoneyChanged);
            gameManager.onHappinessChanged.RemoveListener(OnHappinessChanged);
        }
    }

    private void OnMoneyChanged()
    {
        //Debug.Log("Main UI - Money changed: " + money);
        string newMoneyText = gameData.money.ToString();
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
}
