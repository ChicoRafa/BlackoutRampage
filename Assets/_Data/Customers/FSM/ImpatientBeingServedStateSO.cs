using _Data.Customers.Scripts;
using UnityEngine;

namespace _Data.Customers.FSM {
    [CreateAssetMenu(menuName = "Customers/States/ImpatientBeingServed")]
    public class ImpatientBeingServedStateSO : BeingServedStateSO {

        public override void Tick(Client client) {
            base.Tick(client);
            client.MoveImpatiently();
        }
    }
}