using _Data.Customers.Scripts;
using UnityEngine;

namespace _Data.Customers.FSM
{
    [CreateAssetMenu(menuName = "Customers/States/PoliceLeaveSatisfied")]
    public class PoliceLeaveSatisfiedStateSO : LeaveSatisfiedStateSO
    {
        [Header("Bonus happiness")]
        [SerializeField] private int bonusHappiness = 25;

        public override void OnEnter(Client client)
        {
            base.OnEnter(client);
            client.AddPoliceHappinessBonus(bonusHappiness);
        }
    }
}
