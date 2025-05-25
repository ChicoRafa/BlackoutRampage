using _Data.Customers.FSM;
using UnityEngine;

namespace _Data.Customers.Scriptables
{
    [CreateAssetMenu(fileName = "ClientType", menuName = "Customers/ClientType")]
    public class ClientType : ScriptableObject
    {
        [Header("Config")]
        public string typeName;
        public float baseMinPatience = 100f;
        public float baseMaxPatience = 150f;

        [Header("Visuals")]
        public GameObject modelPrefab;
        public RuntimeAnimatorController animatorController;
        
        [Header("FSM States")]
        public ClientStateSO initialState;
        public ClientStateSO walkingToQueueSlotState;
        public ClientStateSO waitingState;
        public ClientStateSO beingServedState;
        public ClientStateSO leaveSatisfiedState;
        public ClientStateSO leaveAngryState;
    }
}
