using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {

    [Header("Game Settings")]
    [SerializeField] private float levelDurationInMinutes = 8f;

    [Header("Events")]
    public UnityEvent onGameStart;
    public UnityEvent onLevelStart;
    public UnityEvent onLevelEnd;
    public UnityEvent onEveryQuarterPassed;
    public UnityEvent onPause;

    private float levelDuration = 0;
    private float elapsedTime = 0;
    private float quarterDuration;
    private float nextQuarterTime;
    private float passedSeconds = 0;
    private bool levelRunning = false;

    void Awake()
    {
        levelDuration = levelDurationInMinutes * 60;
        quarterDuration = levelDuration / 32;
    }
    void Start()
    {
        onGameStart.Invoke();
        Debug.Log("Game Manager - Game started");
    }

    public void StartLevel()
    {
        elapsedTime = 0f;
        passedSeconds = 0;
        nextQuarterTime = quarterDuration;
        levelRunning = true;
        onLevelStart.Invoke();
        Debug.Log("Game Manager - Level started");
    }

    void Update() {

        if (levelRunning)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= nextQuarterTime)
            {
                onEveryQuarterPassed.Invoke();
                nextQuarterTime += quarterDuration;
            }
            if (elapsedTime - passedSeconds >= 1)
            {
                passedSeconds += 1;
                Debug.Log("Game Manager - Passed seconds: " + passedSeconds);
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
        levelRunning = false;
        Time.timeScale = 0;
        onPause.Invoke();
    }
    
    public void ResumeGame()
    {
        levelRunning = true;
        Time.timeScale = 1;
    }
}
    