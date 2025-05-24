using UnityEngine;
using _Data.Customers.Orders;
using _Data.Customers.Scriptables;
using Unity.VisualScripting;

namespace _Data.Customers.Scripts {
    public class ClientSpawner : MonoBehaviour {
        [Header("Client Prefab & types")]
        public GameObject clientPrefab;
        public ClientType[] clientTypes;

        [Header("References")]
        public Transform spawnPoint;
        public ClientQueueManager queueManager;
        public GameManager gameManager;

        [Header("Order Generation")]
        public ProductCatalog productCatalog;

        [Header("Spawn Rates")]
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
            if (clientPrefab == null || clientTypes.Length == 0 || queueManager == null || spawnPoint == null) return;

            GameObject clientGO = Instantiate(clientPrefab, spawnPoint.position, Quaternion.identity);
            Client client = clientGO.GetComponent<Client>();

            if (client != null) {
                int index = Random.Range(0, clientTypes.Length);
                client.Init(clientTypes[index], queueManager, gameManager);
            }
        }
    }
}