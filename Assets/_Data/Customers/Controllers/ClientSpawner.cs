using UnityEngine;

namespace _Data.Customers.Controllers {
    public class ClientSpawner : MonoBehaviour {
        [Header("Client setup")]
        public GameObject clientPrefab;

        [Header("References")]
        public Transform spawnPoint;
        public ClientQueueManager queueManager;

        public float spawnRate = 5f;
        public int maxClients = 5;

        private float spawnTimer;

        private void Update() {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnRate) {
                spawnTimer = 0f;

                if (queueManager != null && queueManager.CurrentClientCount() < maxClients) {
                    SpawnClient();
                }
            }
        }

        private void SpawnClient() {
            if (clientPrefab == null || queueManager == null || spawnPoint == null) return;

            GameObject clientGO = Instantiate(clientPrefab, spawnPoint.position, Quaternion.identity);
            Client client = clientGO.GetComponent<Client>();

            if (client != null) {
                client.SetQueueManager(queueManager);
                queueManager.EnqueueClient(client);
            }
        }
    }
}