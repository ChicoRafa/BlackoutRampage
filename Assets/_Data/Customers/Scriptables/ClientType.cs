using UnityEngine;

namespace _Data.Customers.Scriptables {
    [CreateAssetMenu(fileName = "ClientType", menuName = "Customer/ClientType")]
    public class ClientType : ScriptableObject {
        public string typeName;
        public float baseMinPatience = 5f;
        public float baseMaxPatience = 10f;
        public Color bodyColor = Color.white;
    }
}