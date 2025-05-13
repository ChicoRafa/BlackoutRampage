using System.Collections.Generic;
using UnityEngine;
using _Data.Products;

namespace _Data.Customers.Orders {
    public static class OrderGenerator {
        private static List<Product> availableProducts = new List<Product>();

        public static void Initialize(List<Product> products) {
            availableProducts = products;
        }

        public static Order GenerateRandomOrder() {
            if (availableProducts == null || availableProducts.Count == 0) {
                Debug.LogError("🚫 OrderGenerator: No available products!");
                return null;
            }

            int itemCount = Random.Range(1, 3);
            List<ItemRequest> items = new List<ItemRequest>();

            for (int i = 0; i < itemCount; i++) {
                Product product = availableProducts[Random.Range(0, availableProducts.Count)];
                int quantity = Random.Range(1, 4);
                items.Add(new ItemRequest(product, quantity));
            }

            return new Order(items);
        }
    }
}