using _Data.Customers.Scripts;
using UnityEngine;

namespace _Data.Customers.FSM {
    [CreateAssetMenu(menuName = "Customers/States/Waiting")]
    public class WaitingStateSO : ClientStateSO {
        public override void OnEnter(Client client) {
            if (!client.GetPatienceController().IsActive())
            {
                client.StartPatience(() =>
                {
                    client.GetFSM().TransitionTo(client.LeaveAngryState);
                });
            }
        }

        public override void OnExit(Client client) {}
    }
}