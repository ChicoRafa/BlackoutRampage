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

    [Header("Interaction Zone")]
    [SerializeField] private InteractionZone interactionZone;

    [Header("Can drop")]
    [HideInInspector] public bool canDrop; 

    private void Start()
    {
        selectedSlot = 0;
        UpdateSelectedSlot(selectedSlot);
        canDrop = true;
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
        if (itemsInInventory[selectedSlot] == null || !canDrop)
            return;

        if (interactionZone.shelving)
        {
            if (itemsInInventory[selectedSlot].GetComponent<ProductScript>().objectType == interactionZone.shelving.objectType)
            {
                for (int i = 0; i < interactionZone.shelving.objectsList.Count; i++)
                {
                    if (interactionZone.shelving.objectsList[i] == null)
                    {
                        if (playerInventorySlots[selectedSlot] == null)
                            return;
                        playerInventorySlots[selectedSlot].sprite = null;
                        playerInventorySlots[selectedSlot].gameObject.SetActive(false);

                        itemsInInventory[selectedSlot].transform.parent = itemsInInventory[selectedSlot].GetComponent<ProductScript>().objectsParent;
                        itemsInInventory[selectedSlot].transform.position = interactionZone.shelving.objectsPositionsList[i].position;
                        interactionZone.shelving.objectsList[i] = itemsInInventory[selectedSlot].gameObject;
                        itemsInInventory[selectedSlot] = null;

                        return;
                    }
                }
            }
            else
                return;
        }
        else
        {
            if (playerInventorySlots[selectedSlot] == null)
                return;
            playerInventorySlots[selectedSlot].sprite = null;
            playerInventorySlots[selectedSlot].gameObject.SetActive(false);

            itemsInInventory[selectedSlot].transform.position = DropSpot.position;
            itemsInInventory[selectedSlot] = null;
        }
    }
    
    public void ClearSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= itemsInInventory.Count) return;

        if (playerInventorySlots[slotIndex] != null)
        {
            playerInventorySlots[slotIndex].sprite = null;
            playerInventorySlots[slotIndex].gameObject.SetActive(false);
        }

        itemsInInventory[slotIndex] = null;
    }
    
    public int GetSelectedSlot()
    {
        return selectedSlot;
    }
}
