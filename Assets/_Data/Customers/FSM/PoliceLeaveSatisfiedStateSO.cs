using _Data.Customers.Scripts;
using UnityEngine;

namespace _Data.Customers.FSM {
    [CreateAssetMenu(menuName = "Customers/States/PoliceLeaveSatisfied")]
    public class PoliceLeaveSatisfiedStateSO : LeaveSatisfiedStateSO {
        [SerializeField] private int bonusHappiness = 25;

        public override void OnEnter(Client client) {
            client.AddPoliceHappinessBonus(bonusHappiness);
            base.OnEnter(client);
        }
    }
}