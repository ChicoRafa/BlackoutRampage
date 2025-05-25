using _Data.Customers.Scripts;
using UnityEngine;

namespace _Data.Customers.FSM
{
    [CreateAssetMenu(menuName = "Customers/States/BeingServed")]
    public class BeingServedStateSO : ClientStateSO
    {
        public override void OnEnter(Client client)
        {
            if (!client.GetPatienceController().IsActive())
            {
                client.StartPatience(() =>
                {
                    client.GetFSM().TransitionTo(client.LeaveAngryState);
                });
            }
        }

        public override void Tick(Client client)
        {
            if (client.HasReceivedInvalidItem())
            {
                client.ClearInteractionFlags();
                client.GetFSM().TransitionTo(client.LeaveAngryState);
            } 
            else if (client.HasReceivedValidItem())
            {
                client.ClearInteractionFlags();
                if (client.CurrentOrder.GetRemainingCount() == 0)
                    client.GetFSM().TransitionTo(client.LeaveSatisfiedState);
            }
        }
        
        public override void OnExit(Client client) { }
    }
}
