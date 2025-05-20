using System.Collections.Generic;
using _Data.Customers.Controllers;
using _Data.Customers.Orders;
using _Data.Products;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Header("Queue Manager")]
    public ClientQueueManager queueManager;

    [Header("Game Settings")]
    [SerializeField] private float levelDurationInMinutes = 8f;

    [Header("Events")]
    public UnityEvent onLevelStart;
    public UnityEvent onLevelEnd;
    public UnityEvent onEveryQuarterPassed;

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
        onLevelEnd.Invoke();
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


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            List<Client> clients = queueManager.GetClientsInServiceSlots();
            foreach (var client in clients)
            {
                if (client.IsReadyToBeServed())
                {
                    Debug.Log("🧪 Simulating interaction with service slot client");
                    client.OnInteractSimulated();
                    return;
                }
            }

            Debug.LogWarning("❌ No client currently ready to be served");
        }

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
                Debug.Log("Level time: " + passedSeconds + " seconds");
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
    }
    
    public void ResumeGame()
    {
        levelRunning = true;
        Time.timeScale = 1;
    }
}
    