using _Data.Customers.Scripts;
using UnityEngine;

namespace _Data.Customers.FSM
{
    [CreateAssetMenu(menuName = "Customers/States/WalkingToQueueSlot")]
    public class WalkingToQueueSlotStateSO : ClientStateSO
    {
        public override void OnEnter(Client client)
        {
            var queueManager = client.GetQueueManager();
            var position = queueManager.GetAssignedSlotFor(client);
            bool isServiceSlot = queueManager.IsClientInServiceSlot(client);

            client.GetMovement().MoveTo(position, () => {
                if (isServiceSlot)
                {
                    client.LookForward();
                    client.GetFSM().TransitionTo(client.BeingServedState);
                }
                else
                    client.GetFSM().TransitionTo(client.WaitingState);
            });
        }

        public override void OnExit(Client client) { }
    }
}
