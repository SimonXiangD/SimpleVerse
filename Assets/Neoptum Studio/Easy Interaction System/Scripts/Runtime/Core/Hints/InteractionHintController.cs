using System.Collections.Generic;
using System.Linq;
using EIS.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

namespace EIS.Runtime.Core.Hints
{
    /// <summary>
    /// Singleton class that manages the display of interaction hints in the game.
    /// This class provides methods to show, hide, and manage a pool of reusable hint prefabs.
    /// </summary>
    public class InteractionHintController : Singleton<InteractionHintController>
    {
        /// <summary>
        /// Prefab used as the base for interaction hint UI elements.
        /// This prefab should contain Text and Image components for displaying hint text and icon.
        /// </summary>
        [Tooltip("Prefab used as the base for interaction hint UI elements.\n " +
                 "This prefab should contain Text and Image components for displaying hint text and icon")]
        [SerializeField]
        private CachedComponentProvider prefab;

        /// <summary>
        /// List of predefined InteractionHintSprite objects for different hint types.
        /// Each InteractionHintSprite defines the sprite to be used for a specific hint type.
        /// </summary>
        [Tooltip("List of predefined InteractionHintSprite objects for different hint types\n" +
                 "Each InteractionHintSprite defines the sprite to be used for a specific hint type.")]
        [SerializeField]
        private List<InteractionHintSprite> sprites = new List<InteractionHintSprite>();

        /// <summary>
        /// Transform that acts as the parent container for all spawned interaction hint UI elements.
        /// </summary>
        [Tooltip("Transform that acts as the parent container for all spawned interaction hint UI elements")]
        [SerializeField]
        private Transform parentContent;

        /// <summary>
        /// Dictionary that stores currently active interaction hints, identified by a HintIdentifier.
        /// HintIdentifier combines hint type and instance ID to uniquely identify a hint.
        /// </summary>
        private Dictionary<HintIdentifier, CachedComponentProvider> prefabUsingPool =
            new Dictionary<HintIdentifier, CachedComponentProvider>();

        /// <summary>
        /// List that stores a pool of inactive interaction hint UI elements.
        /// These elements are reused when a new hint needs to be shown.
        /// </summary>
        private List<CachedComponentProvider> prefabReservePool = new List<CachedComponentProvider>();

        /// <summary>
        /// Checks if there's currently a visible hint for the specified instance ID and hint type.
        /// </summary>
        /// <param name="instanceID">The unique identifier of the game object the hint is associated with.</param>
        /// <param name="hintType">The type of the interaction hint (e.g., PRIMARY, SECONDARY).</param>
        /// <returns>True if a hint is currently shown for the given instance ID and hint type, False otherwise.</returns>
        public bool HasHint(int instanceID, HintType hintType = HintType.PRIMARY)
        {
            HintIdentifier hintIdentifier = new HintIdentifier(hintType, instanceID);
            return prefabUsingPool.ContainsKey(hintIdentifier);
        }

        /// <summary>
        /// Shows an interaction hint with the specified text for the given instance ID and hint type.
        /// </summary>
        /// <param name="text">The text to be displayed in the hint.</param>
        /// <param name="instanceID">The unique identifier of the game object the hint is associated with.</param>
        /// <param name="hintType">The type of the interaction hint (e.g., PRIMARY, SECONDARY).</param>
        public void ShowHint(string text, int instanceID, HintType hintType = HintType.PRIMARY)
        {
            CachedComponentProvider poolItem = GetPoolItem(instanceID, hintType);
            poolItem.transform.SetParent(parentContent, false);
            poolItem.gameObject.SetActive(true);

            poolItem.GetComponentByIndex<TMPro.TMP_Text>("text").SetText(text);
            poolItem.GetComponentByIndex<Image>("image").sprite =
                sprites.First(spriteType => spriteType.hintType == hintType).sprite;
        }

        /// <summary>
        /// Hides the interaction hint for the specified instance ID and hint type.
        /// </summary>
        /// <param name="instanceID">The unique identifier of the game object the hint is associated with.</param>
        /// <param name="hintType">The type of the interaction hint (e.g., PRIMARY, SECONDARY).</param>
        public void HideHint(int instanceID, HintType hintType = HintType.PRIMARY)
        {
            FindAndReleasePoolItem(instanceID, hintType);
        }

        /// <summary>
        /// Finds and releases a hint from the active pool based on instance ID and hint type.
        /// </summary>
        /// <param name="instanceID">The instance ID of the hint.</param>
        /// <param name="hintType">The type of the hint (defaults to PRIMARY).</param>
        private void FindAndReleasePoolItem(int instanceID, HintType hintType = HintType.PRIMARY)
        {
            HintIdentifier hintIdentifier = new HintIdentifier(hintType, instanceID);
            if (!prefabUsingPool.ContainsKey(hintIdentifier)) return;

            ReleasePoolItem(hintIdentifier);
        }


        /// <summary>
        /// Releases a used interaction hint back to the reserve pool.
        /// </summary>
        /// <param name="hintIdentifier">The unique identifier of the hint to be released.</param>
        private void ReleasePoolItem(HintIdentifier hintIdentifier)
        {
            prefabUsingPool[hintIdentifier].gameObject.SetActive(false);
            prefabReservePool.Add(prefabUsingPool[hintIdentifier]);
            prefabUsingPool.Remove(hintIdentifier);
        }

        /// <summary>
        /// Gets a CachedComponentProvider instance from the pool for displaying a hint.
        /// </summary>
        /// <param name="instanceID">The unique identifier of the game object the hint is associated with.</param>
        /// <param name="hintType">The type of the interaction hint (e.g., PRIMARY, SECONDARY).</param>
        /// <returns>A CachedComponentProvider instance to be used for displaying the hint.</returns>
        private CachedComponentProvider GetPoolItem(int instanceID, HintType hintType = HintType.PRIMARY)
        {
            HintIdentifier hintIdentifier = new HintIdentifier(hintType, instanceID);
            CachedComponentProvider firstOrDefault;

            if (prefabUsingPool.ContainsKey(hintIdentifier))
            {
                return prefabUsingPool[hintIdentifier];
            }

            if (prefabReservePool.Count > 0)
            {
                firstOrDefault = prefabReservePool.First();
                prefabReservePool.Remove(firstOrDefault);
            }
            else
            {
                firstOrDefault = Instantiate(prefab);
            }

            prefabUsingPool.Add(hintIdentifier, firstOrDefault);
            return firstOrDefault;
        }

        /// <summary>
        /// Hides all interaction hints associated with the specified instance ID.
        /// </summary>
        /// <param name="instanceID">The unique identifier of the game object.</param>
        public void HideAll(int instanceID)
        {
            prefabUsingPool.Where(keyValuePair => keyValuePair.Key.instanceID.Equals(instanceID))
                .ToList()
                .ForEach(keyValuePair => ReleasePoolItem(keyValuePair.Key));
        }
    }
}