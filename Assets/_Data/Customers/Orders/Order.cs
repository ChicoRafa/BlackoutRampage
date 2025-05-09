using System.Collections.Generic;

namespace _Data.Customers.Orders {
    public class Order {
        public List<ItemRequest> Items;

        public Order(List<ItemRequest> items) {
            Items = items;
        }
    }
}