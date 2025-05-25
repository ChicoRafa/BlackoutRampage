using System.Collections.Generic;
using _Data.PowerUps.Scriptables;
using _Data.PowerUps.Scripts;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("PowerUps")]
    [SerializeField] private List<PowerUpSO> powerUps;
    [SerializeField] private float powerUpLifetime = 20f;
    private List<GameObject> activePowerUps = new List<GameObject>();
    private const int maxPowerUps = 2;

    [Header("Spawn")]
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private Vector2 spawnAreaMin, spawnAreaMax; // change area to gameobject containing the spawn area
                                                                 // (in fact it can be a collider inside this object)
    [Header("Game Manager")]
    [SerializeField] private GameManager gameManager;
    
    private void Start()
    {
        InvokeRepeating(nameof(SpawnRandomPowerUp), 2f, spawnInterval);
    }

    private void SpawnRandomPowerUp()
    {
        activePowerUps.RemoveAll(pu => !pu);
        if (activePowerUps.Count >= maxPowerUps) return;
        
        PowerUpSO selectedPowerUp = powerUps[Random.Range(0, powerUps.Count)];

        Vector3 spawnPosition = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            0.5f,
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        GameObject powerUpGO = Instantiate(selectedPowerUp.pickupPrefab, spawnPosition, Quaternion.identity);
        activePowerUps.Add(powerUpGO);
        var pickup = powerUpGO.GetComponentInChildren<PowerUpPickup>();
        pickup.Init(selectedPowerUp, gameManager);
    }
}
