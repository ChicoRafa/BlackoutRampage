using UnityEngine;

namespace _Data.Customers.Controllers {
    public class ClientSpawner : MonoBehaviour {
        [Header("Client")]
        public GameObject clientPrefab;

        [Header("Spawn Point")]
        public Transform spawnPoint;

        private void Start() {
            SpawnClient();
        }

        private void SpawnClient() {
            if (clientPrefab == null || spawnPoint == null) {
                Debug.LogWarning("❌ ClientSpawner: Prefab or spaw point not assigned.");
                return;
            }

            GameObject clientInstance = Instantiate(clientPrefab, spawnPoint.position, Quaternion.identity);
            clientInstance.name = "Client_Test";
        }
    }
}