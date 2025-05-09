using UnityEngine;
using _Data.Products;
using _Data.PlayerController.Scripts;
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

    private void Awake()
    {
        _productName = Product.productName;
        _sellingPrice = Product.sellingPrice;
        _buyingPrice = Product.buyingPrice;
        _sprite = Product.sprite;
        _prefab = Product.prefab;
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