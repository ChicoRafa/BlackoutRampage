using _Data.Customers.Scripts;
using UnityEngine;

namespace _Data.Customers.FSM
{
    [CreateAssetMenu(menuName = "Customers/States/InitialState")]
    public class InitialStateSO : ClientStateSO
    {
        public override void OnEnter(Client client)
        {
            var queueManager = client.GetQueueManager();
            bool canEnterQueue = queueManager.CanClientJoinQueue();

            if (!canEnterQueue)
            {
                client.GetFSM().TransitionTo(client.LeaveAngryState);
                return;
            }

            queueManager.EnqueueClient(client);
        }

        public override void OnExit(Client client) { }
    }
}
