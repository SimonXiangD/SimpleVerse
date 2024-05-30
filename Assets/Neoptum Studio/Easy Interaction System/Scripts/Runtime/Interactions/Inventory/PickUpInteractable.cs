using EIS.Runtime.Core;
using EIS.Runtime.Inventory;
using EIS.Runtime.Sound;
using UnityEngine;

namespace EIS.Runtime.Interactions.Inventory
{
    /// <summary>
    /// An interactable component that allows the player to pick up an item and add it to their inventory.
    /// </summary>
    public class PickUpInteractable : Interactable
    {
        /// <summary>
        /// The InventoryItemData representing the item to be picked up.
        /// </summary>
        [Tooltip("The InventoryItemData representing the item to be picked up")] [SerializeField]
        private InventoryItemData inventoryItemData;

        /// <summary>
        /// Determines whether to play a sound effect when the item is picked up.
        /// </summary>
        [Tooltip("Determines whether to play a sound effect when the item is picked up")] [SerializeField]
        private bool useSound = true;

        /// <summary>
        /// The sound item to play when the item is picked up, if useSound is true.
        /// </summary>
        [Tooltip("The sound item to play when the item is picked up, if useSound is true")] [SerializeField]
        private SoundItem soundItem;


        #region Interaction Setup fields

        public override bool UsePrimaryActionHint() => true;
        public override string GetPrimaryHintText() => "Pick object";

        public override bool UseSecondaryActionHint() => false;
        public override string GetSecondaryHintText() => default;

        public override bool IsUnsubscribeBlocked() => false;

        #endregion


        /// <summary>
        /// Overrides the base OnInteractPrimary method to handle picking up the item.
        /// </summary>
        public override void OnInteractPrimary()
        {
            if (useSound)
                SoundPlayer.Instance.PlayOneShot(transform.position, soundItem);

            InventoryController.AddToInventory(inventoryItemData);
            Destroy(gameObject);
        }
    }
}