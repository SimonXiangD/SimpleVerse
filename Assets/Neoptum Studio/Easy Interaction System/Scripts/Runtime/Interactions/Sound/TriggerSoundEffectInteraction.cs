using System.Collections.Generic;
using EIS.Runtime.Core;
using EIS.Runtime.Core.Hints;
using EIS.Runtime.Sound;
using UnityEngine;

namespace EIS.Runtime.Interactions.Sound
{
    /// <summary>
    /// An interactable component that triggers a sound effect upon interaction.
    /// </summary>
    public class TriggerSoundEffectInteraction : Interactable
    {
        /// <summary>
        /// Enumeration for different ways to play the sound effect.
        /// <b>EffectIndex</b> - Play using a SoundItem.SoundEffectIndex value.
        /// <b>DynamicIndex</b> - Play using a dynamically generated sound index string
        /// <b>AudioClip</b> - Play using a directly assigned AudioClip
        /// </summary>
        public enum PlaySoundBy
        {
            EffectIndex,
            DynamicIndex,
            AudioClip
        }

        /// <summary>
        /// How the sound effect should be played.
        /// </summary>
        [SerializeField] public PlaySoundBy playSoundBy = PlaySoundBy.EffectIndex;

        /// <summary>
        /// The SoundItem.SoundEffectIndex to use when playSoundBy is set to EffectIndex.
        /// </summary>
        [SerializeField] private SoundItem.SoundEffectIndex soundEffectIndex = SoundItem.SoundEffectIndex.DYNAMIC;

        /// <summary>
        /// The sound index string to use when playSoundBy is set to DynamicIndex.
        /// </summary>
        [SerializeField] private string soundIndex;

        /// <summary>
        /// The AudioClip to use when playSoundBy is set to AudioClip.
        /// </summary>
        [SerializeField] public AudioClip audioClip;


        /// <summary>
        /// Determines whether to display interaction hints for this interactable.
        /// </summary>
        [Space] [SerializeField] private bool useHints = false;

        /// <summary>
        /// The text to display for the primary action hint, if useHints is true.
        /// </summary>
        [SerializeField] private string primaryHintText;


        #region Interaction Setup fields

        public override bool UsePrimaryActionHint() => useHints;
        public override string GetPrimaryHintText() => primaryHintText;

        public override bool UseSecondaryActionHint() => false;
        public override string GetSecondaryHintText() => default;

        protected override List<InteractionHintData> GetInputHints()
        {
            return new List<InteractionHintData>()
            {
                new InteractionHintData(HintType.PRIMARY, "Play")
            };
        }

        public override bool IsUnsubscribeBlocked() => false;

        #endregion


        //This is required to view enable/disable checkbox in inspector
        private void OnEnable()
        {
        }

        public override void OnInteractPrimary()
        {
            switch (playSoundBy)
            {
                case PlaySoundBy.EffectIndex:
                    SoundPlayer.Instance.PlayOneShot(transform.position, soundEffectIndex);
                    break;
                case PlaySoundBy.DynamicIndex:
                    SoundPlayer.Instance.PlayOneShot(transform.position, soundIndex);
                    break;
                case PlaySoundBy.AudioClip:
                    SoundPlayer.Instance.PlayOneShot(transform.position, audioClip);
                    break;
            }
        }
    }
}