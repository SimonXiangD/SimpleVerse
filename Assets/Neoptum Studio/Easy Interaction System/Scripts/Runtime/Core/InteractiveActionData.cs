using System;
using EIS.Runtime.Sound;
using UnityEngine;
using UnityEngine.Events;

namespace EIS.Runtime.Core
{
    /// <summary>
    /// Container for data defining an interactive action that can be associated with interactable objects.
    /// </summary>
    [Serializable]
    public class InteractiveActionData
    {
        /// <summary>
        /// Textual hint describing the action, displayed to the player when the interaction is available.
        /// </summary>
        [Tooltip("Textual hint describing the action, displayed to the player when the interaction is available")]
        public string actionHint;

        /// <summary>
        /// UnityEvent that will be invoked when the action is triggered.
        /// </summary>
        [Tooltip("UnityEvent that will be invoked when the action is triggered")]
        public UnityEvent action;

        /// <summary>
        /// Determines whether to play a sound effect when the action is triggered.
        /// </summary>
        [Space] [Tooltip("Determines whether to play a sound effect when the action is triggered")]
        public bool useSound;

        /// <summary>
        /// The sound item to play when the action is triggered, if useSound is true.
        /// </summary>
        [Tooltip("The sound item to play when the action is triggered, if useSound is true")]
        public SoundItem soundItem;
    }
}