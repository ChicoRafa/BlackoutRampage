using System.Collections.Generic;
using UnityEngine;

namespace _Data.Customers.Controllers {
    public class ClientQueueManager : MonoBehaviour {
        [Header("Queue configuration")]
        public List<Transform> queuePositions;
        public Transform exitPoint;

        private List<Client> clientQueue = new List<Client>();

        public void EnqueueClient(Client client) {
            if (clientQueue.Count >= queuePositions.Count) {
                Debug.LogWarning("🚫 Queue is full! Client will leave angry.");
                client.LeaveBecauseQueueIsFull(exitPoint.position);
                return;
            }

            clientQueue.Add(client);
            AssignClientPositions();
        }

        public void DequeueClient(Client client) {
            if (clientQueue.Contains(client)) {
                clientQueue.Remove(client);
                AssignClientPositions();
            }
        }

        private void AssignClientPositions() {
            int queueIndex = 0;

            foreach (var client in clientQueue) {
                if (client.IsLeaving()) continue;

                Vector3 pos = queuePositions[queueIndex].position;
                bool isFront = (queueIndex == 0);
                client.MoveToQueuePosition(pos, isFront, queueIndex);
                queueIndex++;
            }
        }

        public Transform GetExitPoint() => exitPoint;

        public int CurrentClientCount() {
            return clientQueue.Count;
        }
        
        public Client GetFrontClient() {
            if (clientQueue.Count == 0) return null;
            return clientQueue[0];
        }
    }
}