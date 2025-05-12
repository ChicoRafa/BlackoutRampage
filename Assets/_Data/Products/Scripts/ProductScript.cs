using UnityEngine;
using _Data.Products;
using _Data.PlayerController.Scripts;
using UnityEditor;
//using _Data.PlayerInventory;

public class ProductScript : MonoBehaviour, IInteractable
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

    //public enum typeOfObject
    //{
    //    Battery,
    //    Can,
    //    Candle,
    //    Lantern,
    //    Paper,
    //    Radio,
    //    Water
    //}

    //public typeOfObject objectType;


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
        shelving = transform.parent.transform.parent.gameObject.GetComponent<Shelving>();
        objectsParent = transform.parent;
    }

    public void Interact(GameObject interactor)
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

    public string GetInteractionPrompt()
    {
        return "Interact with " + _productName;
    }
}