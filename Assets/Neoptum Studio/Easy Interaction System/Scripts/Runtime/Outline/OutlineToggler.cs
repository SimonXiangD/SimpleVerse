using EIS.Runtime.Interfaces;
using UnityEngine;

namespace EIS.Runtime.Outline
{
    /// <summary>
    /// Toggles the enabled state of a single outline, implementing the ITogglable interface.
    /// </summary>
    public class OutlineToggler : MonoBehaviour, ITogglable
    {
        /// <summary>
        /// The outline to be toggled.
        /// </summary>
        public QuickOutline.Outline Outline;

        /// <summary>
        /// Enables the outline.
        /// </summary>
        public void Enable()
        {
            Outline.enabled = true;
        }

        /// <summary>
        /// Disables the outline.
        /// </summary>
        public void Disable()
        {
            Outline.enabled = false;
        }
    }
}