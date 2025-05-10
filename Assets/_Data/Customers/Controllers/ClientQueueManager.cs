using System.Collections.Generic;
using UnityEngine;

namespace _Data.Customers.Controllers {
    public class ClientQueueManager : MonoBehaviour {
        [Header("Queue Configuration")]
        public List<Transform> queuePositions;

        [Header("Service Slots")]
        public List<Transform> serviceSlots;

        public Transform exitPoint;

        private List<Client> clientQueue = new List<Client>();
        private Dictionary<Transform, Client> activeServiceClients = new Dictionary<Transform, Client>();

        public void EnqueueClient(Client client) {
            if (clientQueue.Count >= queuePositions.Count) {
                Debug.LogWarning("🚫 Queue is full! Client will leave angry.");
                client.LeaveBecauseQueueIsFull(exitPoint.position);
                return;
            }

            clientQueue.Add(client);
            AssignQueuePositions();
            TryServeNextClient();
        }

        public void DequeueClient(Client client) {
            foreach (var kvp in activeServiceClients) {
                if (kvp.Value == client) {
                    activeServiceClients[kvp.Key] = null;
                    break;
                }
            }

            if (clientQueue.Contains(client)) {
                clientQueue.Remove(client);
            }

            AssignQueuePositions();
            TryServeNextClient();
        }

        private void AssignQueuePositions() {
            int queueIndex = 0;
            foreach (var client in clientQueue) {
                if (client.IsLeaving()) continue;

                Vector3 position = queuePositions[queueIndex].position;
                client.MoveToQueuePosition(position, queueIndex, false); // Not in service slot
                queueIndex++;
            }
        }

        private void TryServeNextClient() {
            if (clientQueue.Count == 0) return;

            foreach (Transform slot in serviceSlots) {
                if (!activeServiceClients.ContainsKey(slot) || activeServiceClients[slot] == null) {
                    Client next = clientQueue[0];
                    activeServiceClients[slot] = next;
                    clientQueue.RemoveAt(0);
                    next.MoveToQueuePosition(slot.position, 0, true); // In service slot
                    break;
                }
            }
        }

        public Transform GetExitPoint() => exitPoint;

        public int CurrentClientCount() {
            return clientQueue.Count;
        }

        public List<Client> GetClientsInServiceSlots() {
            List<Client> activeClients = new List<Client>();
            foreach (var pair in activeServiceClients) {
                if (pair.Value != null)
                    activeClients.Add(pair.Value);
            }
            return activeClients;
        }
    }
}
