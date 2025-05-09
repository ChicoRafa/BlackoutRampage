using UnityEngine;

namespace _Data.Products
{
    [CreateAssetMenu(fileName = "Product", menuName = "Product")]
    public class Product : ScriptableObject
    {
        public string productName;
        public float sellingPrice;
        public float buyingPrice;
        public Sprite sprite;
        public GameObject prefab;
    }
}
