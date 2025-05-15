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
        gameManager.onEvery15SecondsPassed.AddListener(OnEvery15SecondsPassed);
        gameManager.onEveryMinutePassed.AddListener(OnEveryMinutePassed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnLevelStart()
    {
        Debug.Log("Level started");
    }

    private void OnLevelEnd()
    {
        Debug.Log("Level ended");
    }

    private void OnEvery15SecondsPassed()
    {
        Debug.Log("15 SECONDS PASSED");
        minutesCycleIndex++;
        if (minutesCycleIndex >= 4)
        {
            minutesCycleIndex = 0;
            currentMinuteText.text = "00";
        }
        else
        {
            int minuteValue = 15 * minutesCycleIndex;
            currentMinuteText.text = minuteValue.ToString("00");
            
        }
        
    }

    private void OnEveryMinutePassed()
    {
        //Debug.Log("Minute passed, updated day time image");
        currentDayTimeImageIndex++;
        if (currentDayTimeImageIndex >= dayTimeSprites.Count)
        {
            currentDayTimeImageIndex = 0;
        }
        currentDayTimeImage.sprite = dayTimeSprites[currentDayTimeImageIndex];

        string hourText = currentHourText.text;
        int hourValue = int.Parse(hourText);
        hourValue++;
        if (hourValue >= 24)
        {
            hourValue = 0;
        }
        currentHourText.text = hourValue.ToString("00");
    }

    private void OnDisable()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.onLevelStart.RemoveListener(OnLevelStart);
            gameManager.onLevelEnd.RemoveListener(OnLevelEnd);
            gameManager.onEvery15SecondsPassed.RemoveListener(OnEvery15SecondsPassed);
            gameManager.onEveryMinutePassed.RemoveListener(OnEveryMinutePassed);
        }
    }
}
