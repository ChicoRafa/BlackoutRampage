using UnityEngine;

namespace _Data.Products
{
    [CreateAssetMenu(fileName = "Product", menuName = "Product")]
    public class Product : ScriptableObject
    {
        public string productName;
        public int sellingPrice;
        public Sprite sprite;
        public GameObject prefab;
    }
}
