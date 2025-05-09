using System.Collections.Generic;
using _Data.Customers.Orders;
using _Data.Products;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Available Products in the Store")]
    public List<Product> productsInStore;
    void Awake()
    {
        OrderGenerator.AvailableProducts = productsInStore;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
