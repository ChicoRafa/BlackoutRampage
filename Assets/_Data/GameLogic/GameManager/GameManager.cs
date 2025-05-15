using System.Collections.Generic;
using _Data.Customers.Controllers;
using _Data.Customers.Orders;
using _Data.Products;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {
    [Header("Queue Manager")]
    public ClientQueueManager queueManager;

    [Header("Events")]
    public UnityEvent onLevelStart;
    public UnityEvent onLevelEnd;
    public UnityEvent onEvery15SecondsPassed;
    public UnityEvent onEveryMinutePassed;

    [Header("Level Settings")]
    [SerializeField] private float levelDurationInMinutes = 8f;
    private float levelDuration = 0;
    private float elapsedTime = 0;
    private float passedSeconds = 0;
    private bool levelEnded = false;


    void Awake()
    {
        levelDuration = levelDurationInMinutes * 60;  
    }
    void Start()
    {
        StartLevel();
    }

    public void StartLevel()
    {
        elapsedTime = 0f;
        levelEnded = false;
        passedSeconds = 0;
        onLevelStart.Invoke();
        //Debug.Log("Level started");
    }

    public void EndLevel()
    {
        levelEnded = true;
        onLevelEnd.Invoke();
        //Debug.Log("Level ended!");
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            List<Client> clients = queueManager.GetClientsInServiceSlots();
            foreach (var client in clients) {
                if (client.IsReadyToBeServed()) {
                    Debug.Log("🧪 Simulating interaction with service slot client");
                    client.OnInteractSimulated();
                    return;
                }
            }

            Debug.LogWarning("❌ No client currently ready to be served");
        }

        if (!levelEnded)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime - passedSeconds >= 1)
            {
                if (passedSeconds > 0)
                {
                    if (passedSeconds % 15 == 0)
                    {
                        onEvery15SecondsPassed.Invoke();
                    }
                    if (passedSeconds % 60 == 0)
                    {
                        onEveryMinutePassed.Invoke();
                    }
                }
                passedSeconds += 1;
                Debug.Log("Level time: " + passedSeconds + " seconds");
            }
            if (passedSeconds >= levelDuration)
            {
                EndLevel();
            }
        }
    }
}
    