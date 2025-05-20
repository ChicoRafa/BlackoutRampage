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
            Dictionary<Product, int> items = new();

            if (availableProducts == null || availableProducts.Count == 0) {
                Debug.LogError("🚫 OrderGenerator: No available products!");
                return null;
            }

            List<Product> shuffledProducts = new List<Product>(availableProducts);
            Shuffle(shuffledProducts);

            int remainingQuantity = Random.Range(1, 4);
            int index = 0;

            while (remainingQuantity > 0 && index < shuffledProducts.Count) {
                Product product = shuffledProducts[index];
                int maxQuantityForThisItem = Mathf.Min(remainingQuantity, 3);
                int quantity = Random.Range(1, maxQuantityForThisItem + 1);

                items.Add(product, quantity);

                remainingQuantity -= quantity;
                index++;
            }

            return new Order(items);
        }

        private static void Shuffle<T>(List<T> list) {
            for (int i = 0; i < list.Count; i++) {
                int randIndex = Random.Range(i, list.Count);
                (list[i], list[randIndex]) = (list[randIndex], list[i]);
            }
        }
    }
}