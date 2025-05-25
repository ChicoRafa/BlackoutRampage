using _Data.Customers.Scripts;
using UnityEngine;

namespace _Data.Customers.FSM
{
    public abstract class ClientStateSO : ScriptableObject
    {
        public abstract void OnEnter(Client client);

        public abstract void OnExit(Client client);
        
        public virtual void Tick(Client client) { }
    }
}
