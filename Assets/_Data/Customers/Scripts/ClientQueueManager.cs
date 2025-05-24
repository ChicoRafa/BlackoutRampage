using System.Collections.Generic;
using UnityEngine;

namespace _Data.Customers.Scripts {
    public class ClientQueueManager : MonoBehaviour {
        [Header("Queue Configuration")]
        public List<Transform> queuePositions;

        [Header("Service Slots")]
        public List<Transform> serviceSlots;
        
        [Header("Perks")]
        public PerksSO perksData;
        public List<Transform> extraServiceSlots;
        
        [Header("Exit Scenario Point")]
        public Transform exitPoint;

        private List<Client> clientQueue = new List<Client>();
        private Dictionary<Transform, Client> activeServiceClients = new Dictionary<Transform, Client>();

        private void Start()
        {
            if (perksData != null && perksData.perkExtraServiceSlots)
            {
                serviceSlots.AddRange(extraServiceSlots);
            } 
        }
        
        public void EnqueueClient(Client client) {
            if (clientQueue.Count >= queuePositions.Count) {
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
    }
}
