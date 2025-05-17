using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] private Image currentDayTimeImage;
    [SerializeField] private TextMeshProUGUI currentHourText;
    [SerializeField] private TextMeshProUGUI currentMinuteText;
    [SerializeField] private List<Sprite> dayTimeSprites;

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
    }

    private void OnLevelStart()
    {
        Debug.Log("Level started");
    }

    private void OnLevelEnd()
    {
        Debug.Log("Level ended");
    }

    private void OnEveryQuarterPassed()
    {
        Debug.Log("A QUARTER PASSED");
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
        Debug.Log("An hour passed");
        passedHours++;
        UpdateHourText();
        if (passedHours % 2 == 0)
        {
            OnEveryTwoHoursPassed();
        }
    }

    private void OnEveryTwoHoursPassed()
    {
        Debug.Log("Two hours passed");
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
        Debug.Log("Updated day time image");
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
        }
    }
}
