namespace EIS.Runtime.Inventory
{
    public class InventoryItem
    {
        public InventoryItemData inventoryItemData;
        public int count;
        public int uiIndex;

        public InventoryItem(InventoryItemData objData)
        {
            this.inventoryItemData = objData;
            this.count = 0;
            this.uiIndex = -1;
        }
    }
}