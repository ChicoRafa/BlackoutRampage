using System.Collections.Generic;
using UnityEngine;
using _Data.Products;

namespace _Data.Customers.Orders {
    public static class OrderGenerator {
        public static List<Product> AvailableProducts = new List<Product>();

        public static Order GenerateRandomOrder() {
            int itemCount = Random.Range(1, 3);
            List<ItemRequest> items = new List<ItemRequest>();

            for (int i = 0; i < itemCount; i++) {
                Product product = AvailableProducts[Random.Range(0, AvailableProducts.Count)];
                int quantity = Random.Range(1, 4);

                items.Add(new ItemRequest(product, quantity));
            }

            return new Order(items);
        }
    }
}