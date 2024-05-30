using EIS.Runtime.Core;
using UnityEngine;

namespace EIS.Runtime.Interactions.Sound
{
    /// <summary>
    /// An interactable component that plays an AudioClip when interacted with.
    /// </summary>
    [RequireComponent(typeof(AudioSource))] // Enforces adding an AudioSource component
    public class AudioPlayerInteraction : Interactable
    {
        /// <summary>
        /// The AudioClip to be played upon interaction.
        /// </summary>
        [SerializeField] private AudioClip AudioClip;


        /// <summary>
        /// Tracks the current playback state (playing or not).
        /// </summary>
        private bool isPlaying;

        /// <summary>
        /// The AudioSource component attached to this GameObject.
        /// </summary>
        private AudioSource audioSource;


        #region Interaction Setup fields

        public override bool UsePrimaryActionHint() => true;
        public override string GetPrimaryHintText() => audioSource.isPlaying ? "Stop" : "Play";

        public override bool UseSecondaryActionHint() => false;
        public override string GetSecondaryHintText() => default;

        public override bool IsUnsubscribeBlocked() => false;

        #endregion


        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Overrides the base OnInteractPrimary method to toggle audio playback.
        /// </summary>
        public override void OnInteractPrimary()
        {
            if (audioSource.clip == null)
            {
                audioSource.clip = AudioClip;
            }

            if (audioSource.isPlaying)
                audioSource.Stop();
            else
                audioSource.Play();
        }
    }
}