using _Data.Customers.Scripts;
using UnityEngine;

namespace _Data.Customers.FSM {
    [CreateAssetMenu(menuName = "Customers/States/PoliceWaiting")]
    public class PoliceWaitingStateSO : WaitingStateSO {
        [SerializeField, Range(0f, 1f)] private float chanceToLeave = 0.25f;

        public override void OnEnter(Client client) {
            base.OnEnter(client);

            if (Random.value < chanceToLeave) {
                client.PoliceCalledAway();
            }
        }
    }
}