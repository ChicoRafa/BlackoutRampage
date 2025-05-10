using UnityEngine;

namespace _Data.Customers.Controllers {
    public class ClientSpawner : MonoBehaviour {
        [Header("Client Types")]
        public GameObject[] clientPrefabs;

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
                    SpawnRandomClient();
                }
            }
        }

        private void SpawnRandomClient() {
            if (clientPrefabs.Length == 0 || queueManager == null || spawnPoint == null) return;

            int index = Random.Range(0, clientPrefabs.Length);
            GameObject clientGO = Instantiate(clientPrefabs[index], spawnPoint.position, Quaternion.identity);
            Client client = clientGO.GetComponent<Client>();

            if (client != null) {
                client.SetQueueManager(queueManager);
                queueManager.EnqueueClient(client);
            }
        }
    }
}