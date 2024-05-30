using EIS.Runtime.Core;
using EIS.Runtime.Extensions;
using EIS.Runtime.Sound;
using EIS.Runtime.States;
using UnityEngine;

namespace EIS.Runtime.Interactions.Actions
{
    /// <summary>
    /// An interactable component that supports different actions based on its current state (On or Off).
    /// </summary>
    public class StateActionInteraction : Interactable
    {
        /// <summary>
        /// Data for the primary interactive action to be performed when the state is On.
        /// </summary>
        [Tooltip("Data for the primary interactive action to be performed when the state is On")] [SerializeField]
        private InteractiveActionData primaryInteractiveActionWhenOn = new InteractiveActionData();

        /// <summary>
        /// Data for the primary interactive action to be performed when the state is Off.
        /// </summary>
        [Tooltip("Data for the primary interactive action to be performed when the state is Off")] [SerializeField]
        private InteractiveActionData primaryInteractiveActionWhenOff = new InteractiveActionData();

        /// <summary>
        /// The current activity state of the interaction (On or Off).
        /// </summary>
        private ActivityState activityState = ActivityState.Off;


        #region Interaction Setup fields

        public override bool UsePrimaryActionHint() => !GetCurrentPrimaryAction().actionHint.IsNullOrEmpty();

        public override string GetPrimaryHintText() => GetCurrentPrimaryAction().actionHint;

        public override bool UseSecondaryActionHint() => false;
        public override string GetSecondaryHintText() => default;

        public override bool IsUnsubscribeBlocked() => false;

        #endregion


        public override void OnInteractPrimary()
        {
            InteractiveActionData currentAction = GetCurrentPrimaryAction();
            currentAction.action?.Invoke();
            ChangeState(activityState == ActivityState.On ? ActivityState.Off : ActivityState.On);

            if (currentAction.useSound)
                SoundPlayer.Instance.PlayOneShot(transform.position, currentAction.soundItem);
        }

        private InteractiveActionData GetCurrentPrimaryAction()
        {
            if (activityState == ActivityState.On)
                return primaryInteractiveActionWhenOn;

            return primaryInteractiveActionWhenOff;
        }


        /// <summary>
        /// Updates the activity state with the provided value and calls UpdateHints.
        /// </summary>
        /// <param name="activityState">The new activity state to set.</param>
        private void ChangeState(ActivityState activityState)
        {
            this.activityState = activityState;
            UpdateHints();
        }
    }
}