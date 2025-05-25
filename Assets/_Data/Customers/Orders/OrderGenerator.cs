using System.Collections.Generic;
using UnityEngine;
using _Data.Products;

namespace _Data.Customers.Orders
{
    public static class OrderGenerator
    {
        private static List<Product> availableProducts = new List<Product>();

        public static void Initialize(List<Product> products)
        {
            availableProducts = products;
        }

        public static Order GenerateRandomOrder()
        {
            if (availableProducts == null || availableProducts.Count == 0)
                return null;

            List<Product> items = new();
            List<Product> shuffledProducts = new List<Product>(availableProducts);
            Shuffle(shuffledProducts);

            int remainingQuantity = Random.Range(1, 4);

            while (remainingQuantity > 0)
            {
                int randomIndex = Random.Range(0, shuffledProducts.Count); 
                Product product = shuffledProducts[randomIndex];
                items.Add(product);
                remainingQuantity--;
            }

            return new Order(items);
        }

        private static void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randIndex = Random.Range(i, list.Count);
                (list[i], list[randIndex]) = (list[randIndex], list[i]);
            }
        }
    }
}
