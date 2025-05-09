using UnityEngine;
using _Data.Customers.Orders;

namespace _Data.Customers.Controllers {
    public class Client : MonoBehaviour {
        public Order CurrentOrder;

        void Start() {
            CurrentOrder = OrderGenerator.GenerateRandomOrder();
            Debug.Log($"🛒 New order for {gameObject.name}:");

            foreach (var item in CurrentOrder.Items) {
                Debug.Log($" - {item.Product.productName} x{item.Quantity}");
            }
        }
    }
}