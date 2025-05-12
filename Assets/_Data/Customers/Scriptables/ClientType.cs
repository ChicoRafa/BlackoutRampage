using UnityEngine;

namespace _Data.Customers.Scriptables {
    [CreateAssetMenu(fileName = "ClientType", menuName = "Customer/ClientType")]
    public class ClientType : ScriptableObject {
        [Header("Config")]
        public string typeName;
        public float baseMinPatience = 5f;
        public float baseMaxPatience = 10f;

        [Header("Visuals")]
        public GameObject modelPrefab;
        public RuntimeAnimatorController animatorController;
    }
}