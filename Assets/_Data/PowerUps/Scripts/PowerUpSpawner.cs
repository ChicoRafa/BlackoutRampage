using System.Collections.Generic;
using _Data.PowerUps.Scriptables;
using _Data.PowerUps.Scripts;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private List<PowerUpSO> powerUps;
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private Vector2 spawnAreaMin, spawnAreaMax;//change area to gameobject containing the spawn area (in fact it can be a collider inside this object)
    [SerializeField] private GameManager gameManager;
    
    private void Start()
    {
        InvokeRepeating(nameof(SpawnRandomPowerUp), 2f, spawnInterval);
    }

    void SpawnRandomPowerUp()
    {
        PowerUpSO selectedPowerUp = powerUps[Random.Range(0, powerUps.Count)];

        Vector3 spawnPosition = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            0.5f,
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        GameObject powerUpGO = Instantiate(selectedPowerUp.pickupPrefab, spawnPosition, Quaternion.identity);
        var pickup = powerUpGO.GetComponentInChildren<PowerUpPickup>();
        pickup.Init(selectedPowerUp, gameManager);
    }
}
