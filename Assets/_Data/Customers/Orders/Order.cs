using System.Collections.Generic;
using _Data.Products;

namespace _Data.Customers.Orders {
    public class Order {
        private Dictionary<Product, int> remainingItems;
        private int totalOriginalCount;
        private int totalRemainingCount;

        public Order(List<Product> productList) {
            remainingItems = new Dictionary<Product, int>();
            totalOriginalCount = 0;
            totalRemainingCount = 0;

            foreach (var product in productList) {
                if (remainingItems.ContainsKey(product))
                    remainingItems[product]++;
                else
                    remainingItems[product] = 1;

                totalOriginalCount++;
                totalRemainingCount++;
            }
        }

        public bool ContainsProduct(Product product) => remainingItems.ContainsKey(product);

        public bool RemoveProduct(Product product) {
            if (!remainingItems.ContainsKey(product)) return false;

            remainingItems[product]--;
            totalRemainingCount--;

            if (remainingItems[product] <= 0)
                remainingItems.Remove(product);

            return true;
        }

        public int GetRemainingCount() => totalRemainingCount;
        public int GetOriginalCount() => totalOriginalCount;

        public Dictionary<Product, int> GetItemsDict() => remainingItems;

        public List<Product> GetItemsFlatList() {
            var result = new List<Product>();
            foreach (var kvp in remainingItems) {
                for (int i = 0; i < kvp.Value; i++) {
                    result.Add(kvp.Key);
                }
            }
            return result;
        }
    }
}