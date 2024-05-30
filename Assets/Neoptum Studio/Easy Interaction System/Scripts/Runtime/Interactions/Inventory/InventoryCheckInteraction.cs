using System.Collections.Generic;
using EIS.Runtime.Core;
using EIS.Runtime.Inventory;
using EIS.Runtime.UI.Notification;
using UnityEngine;

namespace EIS.Runtime.Interactions.Inventory
{
    /// <summary>
    /// An interactable component that checks for a specific item in the player's inventory before enabling other interactions.
    /// </summary>
    public class InventoryCheckInteraction : Interactable
    {
        /// <summary>
        /// The InventoryItemData representing the item required for interaction.
        /// </summary>
        [Tooltip("The InventoryItemData representing the item required for interaction")] [SerializeField]
        private InventoryItemData requiredItem;

        /// <summary>
        /// Determines whether to remove the required item from the inventory after successful interaction.
        /// </summary>
        [Tooltip("Determines whether to remove the required item from the inventory after successful interaction")]
        [SerializeField]
        private bool removeAfterUse;

        /// <summary>
        /// A list of interactables that should be enabled/disabled based on inventory check results.
        /// </summary>
        [Tooltip("A list of interactables that should be enabled/disabled based on inventory check results")]
        [SerializeField]
        private List<Interactable> interactables = new List<Interactable>();

        /// <summary>
        /// Determines whether the interaction has already been successfully used.
        /// </summary>
        private bool wasInteracted = false;


        #region Interaction Setup fields

        public override bool UsePrimaryActionHint() => !CanInteract();
        public override string GetPrimaryHintText() => "Use";

        public override bool UseSecondaryActionHint() => false;
        public override string GetSecondaryHintText() => default;

        public override bool IsUnsubscribeBlocked() => false;

        #endregion


        private void Start()
        {
            interactables
                .ForEach(interactable => interactable.enabled = false);
        }

        /// <summary>
        /// Overrides the base Subscribe method to enable/disable linked interactables based on inventory check and subscribe them if enabled.
        /// </summary>
        public override void Subscribe()
        {
            bool hasInInventory = CanInteract();
            interactables
                .ForEach(interactable => interactable.enabled = hasInInventory);

            if (hasInInventory)
            {
                interactables.ForEach(interactable => interactable.Subscribe());
            }

            base.Subscribe();
        }

        /// <summary>
        /// Overrides the base OnInteractPrimary method to handle inventory check and item removal.
        /// </summary>
        public override void OnInteractPrimary()
        {
            if (!CanInteract())
            {
                Notifier.Builder.CreateNew()
                    .SetIcon("failed")
                    .SetText($"Find {requiredItem.publicName}")
                    .SetNotificationType(Notifier.NotificationType.LEFT_PANEL)
                    .ShowNotification();
                return;
            }

            wasInteracted = true;
            if (removeAfterUse)
            {
                InventoryController.DeleteFromInventory(requiredItem);
            }
        }

        /// <summary>
        /// Determines whether the interaction can be performed based on inventory state or previous successful interaction.
        /// </summary>
        /// <returns>True if the interaction can be performed, false otherwise.</returns>
        private bool CanInteract()
        {
            return wasInteracted || InventoryController.HasInInventory(requiredItem);
        }
    }
}