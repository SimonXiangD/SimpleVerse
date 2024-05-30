using System.Collections.Generic;
using System.Linq;
using EIS.Runtime.Misc;
using EIS.Runtime.UI.Notification;

namespace EIS.Runtime.Inventory
{
    public class InventoryController : Singleton<InventoryController>
    {
        /// <summary>
        /// Internal list to store all inventory items.
        /// </summary>
        private static List<InventoryItem> inventoryItems = new List<InventoryItem>();

        /// <summary>
        /// Adds a new item or increments the count of an existing item in the inventory.
        /// </summary>
        /// <param name="objData">Data of the inventory item to add.</param>
        public static void AddToInventory(InventoryItemData objData)
        {
            InventoryItem inventoryItem = TryGetInventoryObjectData(objData);
            if (inventoryItem == null)
                inventoryItem = GenerateObjectData(objData);

            inventoryItem.count += 1;

            ShowSuccessNotification(objData);
        }

        /// <summary>
        /// Removes an item from the inventory if it exists.
        /// </summary>
        /// <param name="objData">Data of the inventory item to remove.</param>
        public static void DeleteFromInventory(InventoryItemData objData)
        {
            if (HasInInventory(objData))
            {
                inventoryItems.Remove(TryGetInventoryObjectData(objData));
                ShowRemovedNotification(objData);
            }
        }

        /// <summary>
        /// Checks if an item with the specified data exists in the inventory.
        /// </summary>
        /// <param name="objData">Data of the inventory item to check.</param>
        /// <returns>True if the item exists, False otherwise.</returns>
        public static bool HasInInventory(InventoryItemData objData)
        {
            InventoryItem inventoryItem = TryGetInventoryObjectData(objData);
            return inventoryItem != null;
        }

        /// <summary>
        /// Generates a new InventoryItem object based on the provided data and adds it to the inventory.
        /// </summary>
        /// <param name="objData">Data of the inventory item to create.</param>
        /// <returns>The newly created InventoryItem object.</returns>
        private static InventoryItem GenerateObjectData(InventoryItemData objData)
        {
            InventoryItem inventoryItem = new InventoryItem(objData);
            inventoryItems.Add(inventoryItem);
            return inventoryItem;
        }

        /// <summary>
        /// Tries to find an existing InventoryItem object based on the provided data.
        /// </summary>
        /// <param name="objData">Data of the inventory item to search for.</param>
        /// <returns>The found InventoryItem object or null if not found.</returns>
        private static InventoryItem TryGetInventoryObjectData(InventoryItemData objData)
        {
            return inventoryItems
                .FirstOrDefault(obj => obj.inventoryItemData == objData);
        }

        /// <summary>
        /// Shows a notification indicating an item has been collected.
        /// </summary>
        /// <param name="inventoryItemData">Data of the collected inventory item.</param>
        private static void ShowSuccessNotification(InventoryItemData inventoryItemData)
        {
            Notifier.Builder.CreateNew()
                .SetIcon("collected")
                .SetNotificationType(Notifier.NotificationType.LEFT_PANEL)
                .SetText($"Collected {inventoryItemData.publicName}")
                .ShowNotification();
        }

        /// <summary>
        /// Shows a notification indicating an item has been removed.
        /// </summary>
        /// <param name="inventoryItemData">Data of the removed inventory item.</param>
        private static void ShowRemovedNotification(InventoryItemData inventoryItemData)
        {
            Notifier.Builder.CreateNew()
                .SetIcon("removed")
                .SetNotificationType(Notifier.NotificationType.LEFT_PANEL)
                .SetText($"Removed {inventoryItemData.publicName}")
                .ShowNotification();
        }

        /// <summary>
        /// Returns a list of all currently stored InventoryItem objects.
        /// </summary>
        /// <returns>A list of InventoryItem objects.</returns>
        public static List<InventoryItem> GetObjectsData()
        {
            return inventoryItems;
        }
    }
}