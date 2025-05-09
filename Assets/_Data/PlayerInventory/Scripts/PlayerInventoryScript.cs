using _Data.PlayerInventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class PlayerInventoryScript : MonoBehaviour
{
    [Header("Inventory")]
    public List<Image> playerInventorySlots;
    public List<GameObject> itemsInInventory;
    [SerializeField] private List<Image> playerInventorySlotsBox;

    [Header("Inventory variables")]
    private int selectedSlot;
    [SerializeField] private Color selectedSlotColor;
    [SerializeField] private Transform DropSpot;

    //[Header("Scriptable Object")]
    //[SerializeField] private PlayerInventory PlayerInventory;

    private void Start()
    {
        selectedSlot = 0;
        UpdateSelectedSlot(selectedSlot);
    }

    public void UpdateSelectedSlot(int slot)
    {
        for (int i = 0; i < playerInventorySlotsBox.Count; i++)
            playerInventorySlotsBox[i].color = Color.white;

        selectedSlot = slot;
        playerInventorySlotsBox[selectedSlot].color = selectedSlotColor;
    }

    public void UpdateInventorySlot(int slot, Sprite sprite)
    {
        playerInventorySlots[slot].gameObject.SetActive(true);
        playerInventorySlots[slot].sprite = sprite;
    }

    public void AddItemToInventory(int slot, GameObject item)
    {
        itemsInInventory[slot] = item;
    }

    public void DropItem()
    {
        if (playerInventorySlots[selectedSlot] != null)
        {
            playerInventorySlots[selectedSlot].sprite = null;
            playerInventorySlots[selectedSlot].gameObject.SetActive(false);

            itemsInInventory[selectedSlot].transform.position = 
            itemsInInventory[selectedSlot].transform.position = DropSpot.position;
        }
    }
}