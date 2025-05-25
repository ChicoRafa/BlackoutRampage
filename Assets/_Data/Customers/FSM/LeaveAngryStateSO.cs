using _Data.Customers.Scripts;
using UnityEngine;

namespace _Data.Customers.FSM {
    [CreateAssetMenu(menuName = "Customers/States/LeaveAngry")]
    public class LeaveAngryStateSO : ClientStateSO {
        public override void OnEnter(Client client) {
            client.ShowAngryEffect();
            client.StartLeaving();

            client.GetMovement().MoveTo(client.GetQueueManager().GetExitPoint().position, () => {
                Destroy(client.gameObject);
            });
        }

        public override void OnExit(Client client) { }
    }
}