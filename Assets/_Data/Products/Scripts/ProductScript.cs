using _Data.PlayerController.Scripts;
using _Data.Products;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProductScript : InteractableBase
{
    [Header("Scriptable Object")]
    [SerializeField] private Product Product;

    [Header("Product characteristics")]
    private string _productName;
    private float _sellingPrice;
    private Sprite _sprite;
    private GameObject _prefab;

    [Header("Shelving")]
    private Shelving shelving;
    [HideInInspector] public Transform objectsParent;

    [Header("Object Type")]
    public string objectType;
    
    [Header("Sound effects")]
    [SerializeField] private SoundManagerSO soundManagerSO;
    [SerializeField] private AudioCueSO pickUpCue;
    
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
        PlayerInventoryScript inventory = interactor.GetComponent<PlayerInventoryScript>();
        PlayerController controller = interactor.GetComponent<PlayerController>();

        if (inventory == null || controller == null)
            return;

        for (int i = 0; i < inventory.playerInventorySlots.Count; i++)
        {
            if (inventory.playerInventorySlots[i].sprite != null)
                continue;

            inventory.UpdateInventorySlot(i, _sprite);
            inventory.AddItemToInventory(i, gameObject);
            PlayPickUpSound();

            gameObject.transform.position = controller.objectsTPSpot.position;

            if (objectsParent != null && objectsParent == transform.parent)
                transform.parent = null;


            if (interactor.GetComponent<PlayerController>().storage != null)
            {
                Truck truck = interactor.GetComponent<PlayerController>().storage.GetComponent<Truck>();
                for (int k = 0; k < truck.productsBroughtList.Count; k++)
                {
                    if (truck.productsBroughtList[k] == gameObject)
                    {
                        truck.productsBroughtList[k] = null;

                        bool allNull = truck.productsBroughtList.All(item => item == null);
                        if (allNull)
                            truck.ResetTruckTimer();

                        break;
                    }
                }
            }

            for (int j = 0; j < shelving.objectsList.Count; j++)
            {
                if (shelving.objectsList[j] == gameObject)
                {
                    shelving.objectsList[j] = null;
                    break;
                }
            }

            return;
        }
    }

    private void PlayPickUpSound()
    {
        soundManagerSO.PlaySFX(pickUpCue, "PickUp", 1f);
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
