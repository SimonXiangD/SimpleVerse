using UnityEngine;

namespace EIS.Runtime.Inventory
{
    [CreateAssetMenu(fileName = "Inventory Item Data", menuName = "Inventory/InventoryItemData", order = 1)]
    public class InventoryItemData : ScriptableObject
    {
        public string id;
        
        [Tooltip("Name that will be used in notifications, and other parts in UI")]
        public string publicName;

        public Sprite icon;
    }
}