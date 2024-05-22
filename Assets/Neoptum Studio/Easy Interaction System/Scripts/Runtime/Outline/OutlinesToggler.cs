using System.Collections.Generic;
using EIS.Runtime.Interfaces;
using UnityEngine;

namespace EIS.Runtime.Outline
{
    /// <summary>
    /// Toggles the enabled state of a group of outlines, implementing the ITogglable interface.
    /// </summary>
    public class OutlinesToggler : MonoBehaviour, ITogglable
    {
        /// <summary>
        /// List of outlines to be toggled.
        /// </summary>
        public List<QuickOutline.Outline> Outlines = new List<QuickOutline.Outline>();

        /// <summary>
        /// Enables all outlines in the Outlines list.
        /// </summary>
        public void Enable()
        {
            Outlines.ForEach(outline => outline.enabled = true);
        }

        /// <summary>
        /// Disables all outlines in the Outlines list.
        /// </summary>
        public void Disable()
        {
            Outlines.ForEach(outline => outline.enabled = false);
        }
    }
}