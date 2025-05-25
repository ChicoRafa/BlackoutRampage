using System.Collections.Generic;
using UnityEngine;
using _Data.Products;

namespace _Data.Customers.Scriptables
{
    [CreateAssetMenu(fileName = "ProductCatalog", menuName = "Product/ProductCatalog")]
    public class ProductCatalog : ScriptableObject
    {
        public List<Product> availableProducts;
    }
}
