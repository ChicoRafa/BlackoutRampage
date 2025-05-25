using _Data.Customers.Scripts;
using UnityEngine;

namespace _Data.Customers.FSM
{
    [CreateAssetMenu(menuName = "Customers/States/SantaLeaveSatisfied")]
    public class SantaLeaveSatisfiedStateSO : LeaveSatisfiedStateSO
    {
        [Header("Bonus patience")]
        [SerializeField] private float bonusPatience = 0.3f;

        public override void OnEnter(Client client)
        {
            base.OnEnter(client);

            var allClients = client.GetQueueManager().GetAllActiveClients();

            foreach (var otherClient in allClients)
            {
                if (otherClient != client)
                    otherClient.IncreasePatience(bonusPatience);
            }
        }
    }
}
