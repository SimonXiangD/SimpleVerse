using System.Collections.Generic;
using EIS.Runtime.Core.Hints;
using UnityEngine;

namespace EIS.Runtime.Core
{
    public abstract class Interactable : MonoBehaviour
    {
        /// <summary>
        /// Checks if the game object is enabled.
        /// </summary>
        /// <returns>True if the game object is enabled, false otherwise.</returns>
        public virtual bool IsEnabled() => enabled;

        /// <summary>
        /// Returns the instance ID of the game object.
        /// </summary>
        /// <returns>The instance ID of the game object.</returns>
        public int getInstanceID() => GetInstanceID();

        /// <summary>
        /// Determines whether to use a primary interaction hint for this interactable.
        /// </summary>
        /// <returns>True if a primary interaction hint should be used, false otherwise.</returns>
        public abstract bool UsePrimaryActionHint();

        /// <summary>
        /// Gets the text for the primary interaction hint. 
        /// </summary>
        /// <returns>The text for the primary interaction hint.</returns>
        public abstract string GetPrimaryHintText();

        /// <summary>
        /// Determines whether to use a secondary interaction hint for this interactable.
        /// </summary>
        /// <returns>True if a secondary interaction hint should be used, false otherwise.</returns>
        public abstract bool UseSecondaryActionHint();

        /// <summary>
        /// Gets the text for the secondary interaction hint. 
        /// </summary>
        /// <returns>The text for the secondary interaction hint.</returns>
        public abstract string GetSecondaryHintText();


        /// <summary>
        /// Checks if unsubscribing from the interaction handler is currently blocked.
        /// </summary>
        /// <returns>True if unsubscribing is blocked, false otherwise.</returns>
        public abstract bool IsUnsubscribeBlocked();


        [HideInInspector] public bool isSubscribed;

        /// <summary>
        /// (Protected, Virtual) Gets a list of additional interaction hints for this interactable.
        /// By default, returns null.
        /// </summary>
        /// <returns>A list of additional interaction hints or null if none.</returns>
        protected virtual List<InteractionHintData> GetInputHints()
        {
            return default;
        }

        protected delegate void OnStateChangedEvent();

        protected OnStateChangedEvent onSubscribed;
        protected OnStateChangedEvent onUnsubscribed;

        /// <summary>
        /// Subscribes this interactable to the interaction handler, updates hints, and invokes `onSubscribed` event.
        /// </summary>
        public virtual void Subscribe()
        {
            if (InteractionHandler.Instance)
            {
                InteractionHandler.Instance.Subscribe(this);
            }

            UpdateHints();
            onSubscribed?.Invoke();
        }

        /// <summary>
        /// Unsubscribes this interactable from the interaction handler, hides hints, and invokes `onUnsubscribed` event.
        /// Unsubscription is blocked if `IsUnsubscribeBlocked` returns true.
        /// </summary>
        public void Unsubscribe()
        {
            if (IsUnsubscribeBlocked())
            {
                return;
            }

            if (InteractionHandler.Instance)
            {
                InteractionHandler.Instance.Unsubscribe(this);
            }

            HideHints();
            onUnsubscribed?.Invoke();
        }


        /// <summary>
        /// Shows primary, secondary, and input interaction hints if the interactable is subscribed and the corresponding methods provide data.
        /// </summary>
        public void UpdateHints()
        {
            HideHints();

            if (!isSubscribed)
                return;

            if (!InteractionHintController.Instance)
                return;

            if (UsePrimaryActionHint())
                InteractionHintController.Instance.ShowHint(GetPrimaryHintText(), getInstanceID());


            if (UseSecondaryActionHint())
                InteractionHintController.Instance.ShowHint(GetSecondaryHintText(), getInstanceID(),
                    HintType.SECONDARY);


            List<InteractionHintData> inputHintDtos = GetInputHints();
            if (inputHintDtos == null)
                return;

            inputHintDtos.ForEach(inputHintDto =>
                InteractionHintController.Instance.ShowHint(inputHintDto.text, getInstanceID(),
                    inputHintDto.spriteType));
        }

        /// <summary>
        /// Hides all interaction hints associated with this interactable.
        /// </summary>
        public void HideHints()
        {
            if (InteractionHintController.Instance)
            {
                InteractionHintController.Instance.HideAll(getInstanceID());
            }
        }

        /// <summary>
        /// Invoked when the primary interaction happens on this interactable. 
        /// </summary>
        public virtual void OnInteractPrimary()
        {
        }

        /// <summary>
        /// Invoked when the secondary interaction happens on this interactable. 
        /// </summary>
        public virtual void OnInteractSecondary()
        {
        }

        /// <summary>
        /// (Virtual) Invoked as MonoBehaviour's Update method, but only when the interactable is subscribed.
        /// </summary>
        public virtual void Tick()
        {
        }

        /// <summary>
        /// (Virtual) Invoked as MonoBehaviour's LateUpdate method, but only when the interactable is subscribed.
        /// </summary>
        public virtual void LateTick()
        {
        }

        /// <summary>
        /// (Virtual) Invoked as MonoBehaviour's FixedUpdate method, but only when the interactable is subscribed.
        /// </summary>
        public virtual void FixedTick()
        {
        }

        /// <summary>
        /// Unsubscribes the interactable when it gets disabled.
        /// </summary>
        public virtual void OnDisable()
        {
            Unsubscribe();
        }

        /// <summary>
        /// Overrides the default ToString method to provide a string representation of the interactable with its key properties.
        /// </summary>
        /// <returns>A string representation of the interactable.</returns>
        public override string ToString()
        {
            return $"name: {GetType()}\n\n" +
                   $"enabled: {enabled}\n" +
                   $"usePrimaryActionHint: {UsePrimaryActionHint()}\n" +
                   $"getPrimaryHintText: {GetPrimaryHintText()}\n\n" +
                   $"useSecondaryActionHint: {UseSecondaryActionHint()}\n" +
                   $"getSecondaryHintText: {GetSecondaryHintText()}";
        }
    }
}