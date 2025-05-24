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
            clientQueue.Add(client);
            UpdateQueue();
        }

        public void DequeueClient(Client client) {
            foreach (var kvp in activeServiceClients) {
                if (kvp.Value == client) {
                    activeServiceClients[kvp.Key] = null;
                    break;
                }
            }

            clientQueue.Remove(client);
            UpdateQueue();
        }
        
        public void UpdateQueue() {
            // 1. Try to fill available service slots first
            foreach (Transform slot in serviceSlots) {
                if (!activeServiceClients.ContainsKey(slot) || activeServiceClients[slot] == null) {
                    Client next = clientQueue.Find(c => !c.IsLeaving());

                    if (next != null) {
                        activeServiceClients[slot] = next;
                        clientQueue.Remove(next);
                        next.GetFSM().TransitionTo(next.WalkingToQueueSlotState);
                    }
                }
            }

            // 2. Update the rest of the queue
            int queueIndex = 0;
            foreach (var client in clientQueue) {
                if (client == null || client.IsLeaving()) continue;
                if (IsClientInServiceSlot(client)) continue;

                client.GetFSM().TransitionTo(client.WalkingToQueueSlotState);
                queueIndex++;
            }
        }

        public Transform GetExitPoint() => exitPoint;

        public bool CanClientJoinQueue() {
            return clientQueue.Count < queuePositions.Count;
        }
        public int CurrentClientCount() {
            return clientQueue.Count;
        }

        public bool IsClientInServiceSlot(Client client) {
            foreach (var kvp in activeServiceClients) {
                if (kvp.Value == client) {
                    return true;
                }
            }
            return false;
        }
        
        public int GetClientIndex(Client client) {
            return clientQueue.IndexOf(client);
        }
        
        public Vector3 GetAssignedSlotFor(Client client) {
            foreach (var kvp in activeServiceClients) {
                if (kvp.Value == client) {
                    return kvp.Key.position;
                }
            }

            int index = clientQueue.IndexOf(client);
            if (index >= 0 && index < queuePositions.Count) {
                return queuePositions[index].position;
            }

            return exitPoint != null ? exitPoint.position : Vector3.zero;
        }
    }
}
