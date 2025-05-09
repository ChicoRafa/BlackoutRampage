using _Data.PlayerInventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryScript : MonoBehaviour
{
    [Header("Inventory")]
    public List<Image> playerInventorySlots;

    //[Header("Scriptable Object")]
    //[SerializeField] private PlayerInventory PlayerInventory;

    public void UpdateInventorySlot(int slot, Sprite sprite)
    {
        playerInventorySlots[slot].gameObject.SetActive(true);
        playerInventorySlots[slot].sprite = sprite;
    }
}