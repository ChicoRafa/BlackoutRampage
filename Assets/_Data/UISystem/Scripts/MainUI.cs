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

    [Header("UI Texts")]
    [SerializeField] private TextMeshProUGUI currentHourText;
    [SerializeField] private TextMeshProUGUI currentMinuteText;


    [Header("UI Images")]
    [SerializeField] private Image currentDayTimeImage;
    [SerializeField] private List<Sprite> dayTimeSprites;

    private int currentDayTimeImageIndex = 0;
    private int minutesCycleIndex = 0;
    private int passedHours = 0;
    private TextMeshProUGUI startingHourText;

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
        startingHourText = currentHourText;
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
    }

    private void OnLevelStart()
    {
        //Debug.Log("Main UI - Level started");
        currentDayTimeImage.sprite = dayTimeSprites[0];
        currentHourText.text = startingHourText.text;
        currentMinuteText.text = "00";
    }

    private void OnLevelEnd()
    {
        //Debug.Log("Main UI - Level ended");
    }

    private void OnPause()
    {
        levelMenuObject.SetActive(true);
        inGameElementsObject.SetActive(false);
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
        if (passedHours == 7)
        {
            StartCoroutine(AnimateTimeTextScale());
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
            gameManager.onPause.RemoveListener(OnPause);
        }
    }
    IEnumerator AnimateTimeTextScale()
    {
        Vector3 originalScale = timeTextObject.transform.localScale;
        Vector3 targetScale = originalScale * 2f;
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
}
