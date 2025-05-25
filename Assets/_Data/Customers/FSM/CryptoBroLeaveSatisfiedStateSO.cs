using _Data.Customers.Scripts;
using UnityEngine;

namespace _Data.Customers.FSM
{
    [CreateAssetMenu(menuName = "Customers/States/CryptoBroLeaveSatisfied")]
    public class CryptoBroLeaveSatisfiedStateSO : LeaveSatisfiedStateSO
    {
        [Header("Money bonus")]
        [SerializeField] private int moneyBonus = 200;

        public override void OnEnter(Client client)
        {
            base.OnEnter(client);
            client.AddCryptoMoneyBonus(moneyBonus);
        }
    }
}
