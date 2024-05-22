using System;
using UnityEngine;

namespace EIS.Runtime.Sound
{
    /// <summary>
    /// Container for information about a sound effect, used for organization and flexible referencing.
    /// </summary>
    [Serializable]
    public class SoundItem
    {
        /// <summary>
        /// Determines how to reference the sound effect, either directly by AudioClip or indirectly by soundIndex.
        /// </summary>
        public SoundEffectIndex soundEffectIndex = SoundEffectIndex.DYNAMIC;

        /// <summary>
        /// String identifier for the sound effect, used when soundEffectIndex is set to DYNAMIC.
        /// </summary>
        public string soundIndex;

        /// <summary>
        /// Direct reference to the AudioClip to play, used when soundEffectIndex is not DYNAMIC.
        /// </summary>
        public AudioClip audioClip;

        /// <summary>
        /// Defines different ways to reference sound effects for play.
        /// </summary>
        public enum SoundEffectIndex
        {
            /// <summary>
            /// Indicates the sound effect is referenced by a string soundIndex.
            /// </summary>
            DYNAMIC,

            /// <summary>
            /// Another specific way to reference the sound effect, details likely defined elsewhere.
            /// </summary>
            Another,

            // ... (other potential SoundEffectIndex values)
        }
    }
}