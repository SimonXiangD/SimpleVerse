using EIS.Runtime.Core;
using UnityEngine;

namespace EIS.Runtime.Interactions.Animations
{
    /// <summary>
    /// An interactable component that plays a specific animation on primary interaction.
    /// </summary>
    public class SimpleAnimationInteraction : Interactable
    {
        /// <summary>
        /// Reference to the Animator component to control.
        /// </summary>
        [Tooltip("Reference to the Animator component to control")] [SerializeField]
        private Animator Animator;

        /// <summary>
        /// The name of the animation to play on interaction.
        /// </summary>
        [Tooltip("The name of the animation to play on interaction")] [SerializeField]
        private string animationName;


        #region Interaction Setup fields

        public override bool UsePrimaryActionHint() => true;
        public override string GetPrimaryHintText() => "Use";

        public override bool UseSecondaryActionHint() => false;
        public override string GetSecondaryHintText() => default;

        public override bool IsUnsubscribeBlocked() => false;

        #endregion


        public override void OnInteractPrimary()
        {
            Animator.Play(animationName);
        }
    }
}