using System.Collections.Generic;
using UnityEngine;

namespace _Data.PlayerInventory
{
    [CreateAssetMenu(fileName = "PlayerInventory", menuName = "PlayerInventory")]
    public class PlayerInventory : ScriptableObject
    {
        public List<Sprite> playerInventorySlotImage;
    }
}