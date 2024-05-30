using UnityEngine;

namespace EIS.Runtime.Core.Helpers
{
    /// <summary>
    /// Represents a proxy class for interacting with an Interactable object.
    /// This class serves as a bridge for managing subscriptions and invoking 
    /// interactions on another Interactable component.
    /// </summary>
    public class InteractableProxy : Interactable
    {
        /// <summary>
        /// The GameObject containing the Interactable component to be proxied.
        /// </summary>
        [SerializeField] private GameObject Interactable;

        /// <summary>
        /// A reference to the Interactable component being proxied.
        /// This field is not displayed in the Inspector for clarity.
        /// </summary>
        [HideInInspector] public Interactable iinteractable;

        #region Interaction Setup fields

        public override bool UsePrimaryActionHint() => iinteractable.UsePrimaryActionHint();
        public override string GetPrimaryHintText() => iinteractable.GetPrimaryHintText();
        public override bool UseSecondaryActionHint() => iinteractable.UseSecondaryActionHint();
        public override string GetSecondaryHintText() => iinteractable.GetSecondaryHintText();
        public override bool IsUnsubscribeBlocked() => iinteractable.IsUnsubscribeBlocked();

        #endregion

        /// <summary>
        /// Initializes the InteractableProxy by obtaining a reference to the 
        /// Interactable component and setting up event handlers for subscriptions.
        /// </summary>
        private void Start()
        {
            iinteractable = Interactable.GetComponent<Interactable>();

            // Register event handlers to handle subscription changes
            onSubscribed += OnSubscribedMethod;
            onUnsubscribed += OnUnsubscribedMethod;
        }

        /// <summary>
        /// Handler for the onSubscribed event, called when the InteractableProxy 
        /// is subscribed to. Forwards the subscription to the proxied Interactable.
        /// </summary>
        private void OnSubscribedMethod()
        {
            iinteractable.Subscribe();
        }

        /// <summary>
        /// Handler for the onUnsubscribed event, called when the InteractableProxy 
        /// is unsubscribed from. Forwards the unsubscription to the proxied Interactable.
        /// </summary>
        private void OnUnsubscribedMethod()
        {
            iinteractable.Unsubscribe();
        }
    }
}