using _Data.Products;

namespace _Data.Customers.Orders {
    public struct ItemRequest {
        public Product Product;
        public int Quantity;

        public ItemRequest(Product product, int quantity) {
            Product = product;
            Quantity = quantity;
        }
    }
}