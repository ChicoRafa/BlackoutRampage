using _Data.Customers.Scripts;
using UnityEngine;

namespace _Data.Customers.FSM {
    [CreateAssetMenu(menuName = "Customers/States/KarenLeaveAngry")]
    public class KarenLeaveAngryStateSO : LeaveAngryStateSO {
        [SerializeField] private int penaltyHappiness = 500;
        [SerializeField] private float penaltyPatience = 0.3f;
        public override void OnEnter(Client client) {
            base.OnEnter(client);

            var allClients = client.GetQueueManager().GetAllActiveClients();

            foreach (var otherClient in allClients) {
                if (otherClient != client) {
                    otherClient.ReducePatience(penaltyPatience);
                }
            }
            client.AddKarenHappinessPenalty(penaltyHappiness);
            Debug.Log($"💥 {client.name} (Karen) caused chaos! All clients lost patience.");
        }
    }
}