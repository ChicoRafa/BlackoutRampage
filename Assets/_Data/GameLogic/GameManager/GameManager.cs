using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{

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

    [Header("Perks")]
    [SerializeField] private PerksSO perksData;

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
        levelRunning = false;
        Time.timeScale = 0;
        onPause.Invoke();
    }

    public void ResumeGame()
    {
        levelRunning = true;
        Time.timeScale = 1;
    }

    private void CheckPerks()
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
            }
            else if (perksData.perkShelvingLvl3)
            {
                child1.gameObject.SetActive(false);
                child2.gameObject.SetActive(false);
                child3.gameObject.SetActive(true);
            }
        }
    }
}