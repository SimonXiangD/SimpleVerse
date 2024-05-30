using EIS.Runtime.Core;
using EIS.Runtime.Extensions;
using EIS.Runtime.Sound;
using UnityEngine;

namespace EIS.Runtime.Interactions.Actions
{
    /// <summary>
    /// An interactable component that supports primary and secondary interactions using Unity events.
    /// </summary>
    public class UnityActionInteractable : Interactable
    {
        /// <summary>
        /// Determines whether to use the primary interactive action.
        /// </summary>
        [Tooltip("Determines whether to use the primary interactive action")] [SerializeField]
        private bool usePrimaryAction;

        /// <summary>
        /// Data for the primary interactive action to be performed.
        /// </summary>
        [Tooltip("Data for the primary interactive action to be performed")] [SerializeField]
        private InteractiveActionData primaryInteractiveAction = new InteractiveActionData();


        /// <summary>
        /// Determines whether to use the secondary interactive action.
        /// </summary>
        [Tooltip("Determines whether to use the secondary interactive action")] [Space] [SerializeField]
        private bool useSecondaryAction;

        /// <summary>
        /// Data for the secondary interactive action to be performed.
        /// </summary>
        [Tooltip("Data for the secondary interactive action to be performed")] [SerializeField]
        private InteractiveActionData secondaryInteractiveAction = new InteractiveActionData();


        #region Interaction Setup fields

        public override bool UsePrimaryActionHint() =>
            usePrimaryAction && !primaryInteractiveAction.actionHint.IsNullOrEmpty();

        public override string GetPrimaryHintText() => primaryInteractiveAction.actionHint;

        public override bool UseSecondaryActionHint() =>
            useSecondaryAction && !secondaryInteractiveAction.actionHint.IsNullOrEmpty();

        public override string GetSecondaryHintText() => secondaryInteractiveAction.actionHint;

        public override bool IsUnsubscribeBlocked() => false;

        #endregion


        public override void OnInteractPrimary()
        {
            if (usePrimaryAction)
            {
                primaryInteractiveAction.action?.Invoke();

                if (primaryInteractiveAction.useSound)
                    SoundPlayer.Instance.PlayOneShot(transform.position, primaryInteractiveAction.soundItem);
            }
        }

        public override void OnInteractSecondary()
        {
            if (useSecondaryAction)
            {
                secondaryInteractiveAction.action?.Invoke();

                if (secondaryInteractiveAction.useSound)
                    SoundPlayer.Instance.PlayOneShot(transform.position, secondaryInteractiveAction.soundItem);
            }
        }
    }
}