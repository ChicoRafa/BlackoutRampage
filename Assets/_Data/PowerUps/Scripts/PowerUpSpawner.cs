using System.Collections.Generic;
using _Data.PowerUps.Scriptables;
using _Data.PowerUps.Scripts;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private List<PowerUpSO> powerUps;
    [SerializeField] private GameObject powerUpPickupPrefab;
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private Vector2 spawnAreaMin, spawnAreaMax;//change area to gameobject containing the spawn area (in fact it can be a collider inside this object)

    private void Start()
    {
        InvokeRepeating(nameof(SpawnRandomPowerUp), 2f, spawnInterval);
    }

    void SpawnRandomPowerUp()
    {
        Vector3 spawnPosition = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            0.5f,
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        GameObject go = Instantiate(powerUpPickupPrefab, spawnPosition, Quaternion.identity);
        var pickup = go.GetComponent<PowerUpPickup>();
        pickup.SetPowerUp(powerUps[Random.Range(0, powerUps.Count)]);
    }
}
