using UnityEngine;
using _Data.Products;
using _Data.PlayerController.Scripts;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;
//using _Data.PlayerInventory;

public class ProductScript : InteractableBase
{
    [Header("Scriptable Object")]
    [SerializeField] private Product Product;
    //[SerializeField] private PlayerInventory PlayerInventory;

    [Header("Product characteristics")]
    private string _productName;
    private float _sellingPrice;
    private float _buyingPrice;
    private Sprite _sprite;
    private GameObject _prefab;

    [Header("Shelving")]
    private Shelving shelving;
    [HideInInspector] public Transform objectsParent;

    [Header("Object Type")]
    public string objectType;
    
    public enum productOrigin
    {
        Shelving,
        Truck
    }

    public productOrigin origin;


    private void Awake()
    {
        _productName = Product.productName;
        _sellingPrice = Product.sellingPrice;
        _buyingPrice = Product.buyingPrice;
        _sprite = Product.sprite;
        _prefab = Product.prefab;
    }

    private void Start()
    {
        List<Shelving> allShelves = new List<Shelving>(FindObjectsByType<Shelving>(FindObjectsSortMode.None));
        for (int i = 0; i < allShelves.Count; i++)
        {
            if (allShelves[i].objectType == objectType)
            {
                shelving = allShelves[i];
                break;
            }
        }

        if (transform.parent != null)
            objectsParent = transform.parent;
    }

    public override void Interact(GameObject interactor)
    {
        //Debug.Log(interactor.name + " interacted with " + gameObject.name);

        if (interactor.GetComponent<PlayerInventoryScript>() == null)
            return;

        for (int i = 0; i < interactor.GetComponent<PlayerInventoryScript>().playerInventorySlots.Count; i++)
        {
            if (interactor.GetComponent<PlayerInventoryScript>().playerInventorySlots[i].sprite == null)
            {
                interactor.GetComponent<PlayerInventoryScript>().UpdateInventorySlot(i, _sprite);
                interactor.GetComponent<PlayerInventoryScript>().AddItemToInventory(i, gameObject);

                gameObject.transform.position = interactor.GetComponent<PlayerController>().objectsTPSpot.position;
                //gameObject.SetActive(false);

                if (objectsParent != null && objectsParent == transform.parent)
                    transform.parent = null;

                if (interactor.GetComponent<PlayerController>().storage != null)
                {
                    for (int k = 0; k < interactor.GetComponent<PlayerController>().storage.GetComponent<Truck>().productsBroughtList.Count; k++)
                    {
                        if (interactor.GetComponent<PlayerController>().storage.GetComponent<Truck>().productsBroughtList[k] == gameObject)
                        {
                            interactor.GetComponent<PlayerController>().storage.GetComponent<Truck>().productsBroughtList[k] = null;

                            int nulls = 0;
                            for (int l = 0; l < interactor.GetComponent<PlayerController>().storage.GetComponent<Truck>().productsBroughtList.Count; l++)
                            {
                                if (interactor.GetComponent<PlayerController>().storage.GetComponent<Truck>().productsBroughtList[l] == null)
                                {
                                    nulls++;
                                    if (nulls == interactor.GetComponent<PlayerController>().storage.GetComponent<Truck>().productsBroughtList.Count)
                                        interactor.GetComponent<PlayerController>().storage.GetComponent<Truck>().ResetTruckTimer();
                                }
                                    
                            }
                        }
                    }
                }
                    
                for (int j = 0; j < shelving.objectsList.Count; j++)
                {
                    if (shelving.objectsList[j] != null && shelving.objectsList[j] == this.gameObject)
                    {
                        shelving.objectsList[j] = null;
                        return;
                    }
                }

                return;
            }
            if (i == interactor.GetComponent<PlayerInventoryScript>().playerInventorySlots.Count - 1)
                Debug.Log("Player has full inventory");
        }
    }

    public override bool CanInteract(GameObject interactor)
    {
        return true;
    }
    public override string GetInteractionPrompt()
    {
        return "Pick up " + _productName;
    }
    
    public Product GetProduct()
    {
        return Product;
    }
}