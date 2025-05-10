using System.Collections.Generic;
using _Data.Customers.Controllers;
using _Data.Customers.Orders;
using _Data.Products;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [Header("Available Products in the Store")]
    public List<Product> productsInStore;
    [Header("Queue Manager")]
    public ClientQueueManager queueManager;

    void Awake() {
        OrderGenerator.AvailableProducts = productsInStore;
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
    }
}