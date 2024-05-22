using System.Collections.Generic;
using EIS.Runtime.Misc;
using EIS.Runtime.Restrictions;
using EIS.Runtime.Sound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EIS.Runtime.Inventory
{
    public class InventoryView : Singleton<InventoryView>
    {
        /// <summary>
        /// Reference to the GameObject representing the inventory panel UI.
        /// </summary>
        [Tooltip("Reference to the GameObject representing the inventory panel UI")] [SerializeField]
        private GameObject panel;

        /// <summary>
        /// Reference to the Transform within the panel that acts as parent for slot prefabs.
        /// </summary>
        [Tooltip("Reference to the Transform within the panel that acts as parent for slot prefabs.")] [SerializeField]
        private Transform contentParent;

        /// <summary>
        /// Flag indicating whether to play a sound effect when opening the inventory.
        /// </summary>
        [Space] [Tooltip("Flag indicating whether to play a sound effect when opening the inventory.")] [SerializeField]
        private bool useOpenSound = true;

        /// <summary>
        /// Reference to the SoundItem asset for the opening sound effect.
        /// </summary>
        [Tooltip("Reference to the SoundItem asset for the opening sound effect")] [SerializeField]
        private SoundItem soundOpen;

        /// <summary>
        /// Prefab for a single inventory slot UI element.
        /// </summary>
        [Tooltip("Prefab for a single inventory slot UI element.")]
        public CachedComponentProvider slotPrefab;

        /// <summary>
        /// List of all currently instantiated inventory slot prefabs.
        /// </summary>
        private List<CachedComponentProvider> slots = new List<CachedComponentProvider>();

        private void Update()
        {
            CheckInput();
        }

        /// <summary>
        /// Handles user input for toggling the inventory visibility.
        /// </summary>
        private void CheckInput()
        {
            if (!Input.GetButtonDown("ToggleInventory"))
            {
                return;
            }

            if (panel.activeSelf)
            {
                HideInventory();
            }
            else
            {
                ShowInventory();
            }
        }

        /// <summary>
        /// Shows the inventory UI and plays the opening sound if enabled.
        /// </summary>
        private void ShowInventory()
        {
            if (useOpenSound)
                SoundPlayer.Instance.PlayOneShot(transform.position, soundOpen);

            panel.SetActive(true);
            PlayerRestrictions.AddState(PlayerRestrictions.RestrictionState.IsInUI);

            List<InventoryItem> inventoryObjectModels = InventoryController.GetObjectsData();
            for (int i = 0; i < inventoryObjectModels.Count; i++)
            {
                InventoryItem inventoryItem = inventoryObjectModels[i];
                if (inventoryItem.uiIndex == -1)
                {
                    inventoryItem.uiIndex = GetNewEmptySlotIndex();
                }

                FillSlotWithData(inventoryItem);
            }
        }

        /// <summary>
        /// Updates the UI data for a specific inventory slot based on the provided InventoryItem.
        /// </summary>
        /// <param name="inventoryItem">The InventoryItem data to populate the slot with.</param>
        private void FillSlotWithData(InventoryItem inventoryItem)
        {
            CachedComponentProvider slot = slots[inventoryItem.uiIndex];
            TMP_Text slotName = slot.GetComponentByIndex<TMP_Text>("name");
            if (slotName)
                slotName.SetText(inventoryItem.inventoryItemData.publicName);

            TMP_Text slotCount = slot.GetComponentByIndex<TMP_Text>("count");
            if (slotCount)
                slotCount.SetText(inventoryItem.count.ToString());

            Image slotIcon = slot.GetComponentByIndex<Image>("icon");
            if (slotIcon)
                slotIcon.sprite = inventoryItem.inventoryItemData.icon;
        }

        /// <summary>
        /// Hides the inventory UI and removes the "IsInUI" restriction state.
        /// </summary>
        private void HideInventory()
        {
            panel.SetActive(false);
            PlayerRestrictions.RemoveState(PlayerRestrictions.RestrictionState.IsInUI);
        }

        /// <summary>
        /// Creates a new instance of the inventory slot prefab and assigns it an index in the slots list.
        /// </summary>
        /// <returns>The index of the newly created slot in the slots list.</returns>
        private int GetNewEmptySlotIndex()
        {
            CachedComponentProvider cachedComponentProvider = Instantiate(slotPrefab, contentParent, true);
            slots.Add(cachedComponentProvider);
            return slots.Count - 1;
        }
    }
}