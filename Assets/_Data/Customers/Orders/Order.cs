using System.Collections.Generic;
using _Data.Products;

namespace _Data.Customers.Orders {
    public class Order {
        public Dictionary<Product, int> Items;

        public Order(Dictionary<Product, int> items) {
            Items = items;
        }
    }
}