using _Data.Customers.Scripts;
using UnityEngine;

namespace _Data.Customers.FSM
{
    public class ClientFSM : MonoBehaviour
    {
        private ClientStateSO currentState;
        private Client client;

        public void Init(Client client, ClientStateSO initialState)
        {
            this.client = client;
            TransitionTo(initialState);
        }

        public void TransitionTo(ClientStateSO newState)
        {
            if (currentState != null) currentState.OnExit(client);
            currentState = newState;
            if (currentState != null) currentState.OnEnter(client);
        }

        public ClientStateSO GetCurrentState() => currentState;
        
        private void Update()
        {
            currentState?.Tick(client);
        }
    }
}
