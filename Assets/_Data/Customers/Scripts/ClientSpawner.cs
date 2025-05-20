using UnityEngine;
using _Data.Customers.Orders;
using _Data.Customers.Scriptables;

namespace _Data.Customers.Scripts {
    public class ClientSpawner : MonoBehaviour {
        [Header("Client Types")]
        public GameObject[] clientPrefabs;

        [Header("References")]
        public Transform spawnPoint;
        public ClientQueueManager queueManager;

        [Header("Order Generation")]
        public ProductCatalog productCatalog;

        public float spawnRate = 5f;
        public int maxClients = 5;

        private float spawnTimer;

        private void Awake() {
            if (productCatalog != null) {
                OrderGenerator.Initialize(productCatalog.availableProducts);
            } else {
                Debug.LogError("🚫 ClientSpawner: ProductCatalog is not assigned.");
            }
        }

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