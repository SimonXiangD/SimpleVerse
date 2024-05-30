using System.Collections;
using System.Collections.Generic;
using EIS.Runtime.Core;
using EIS.Runtime.Extensions;
using EIS.Runtime.Restrictions;
using EIS.Runtime.Sound;
using EIS.Runtime.States;
using UnityEngine;

namespace EIS.Runtime.Interactions.ItemInteraction
{
    public class CarryableInteraction : Interactable
    {
        /// <summary>
        /// The distance at which the object is held when examined.
        /// </summary>
        [Tooltip("The distance at which the object is held when examined")] [SerializeField]
        private float examineDistance = 0.3f;

        /// <summary>
        /// An offset applied to the position when held.
        /// </summary>
        [Tooltip("An offset applied to the position when held")] [Space] [SerializeField]
        private Vector3 moveOffset = Vector3.zero;

        /// <summary>
        /// The transform to attach the object to when picked up.
        /// Leave empty to use player's root.
        /// </summary>
        [Tooltip("The transform to attach the object to when picked up. Leave empty to use player's root")]
        [SerializeField]
        private UnityEngine.Transform attachPoint;

        /// <summary>
        /// Flags whether to play a sound when picked up.
        /// </summary>
        [Tooltip("Flags whether to play a sound when picked up")] [Space] [SerializeField]
        private bool useSound = true;

        /// <summary>
        /// The sound item to play when picked up.
        /// </summary>
        [Tooltip("The sound item to play when picked up")] [SerializeField]
        private SoundItem soundItem;

        /// <summary>
        /// A list of interactables to activate when this object is held.
        /// </summary>
        [Space] [Tooltip("A list of interactables to activate when this object is held")] [SerializeField]
        private List<Interactable> interactables = new List<Interactable>();

        /// <summary>
        /// The transform of the GameObject this script is attached to.
        /// </summary>
        private UnityEngine.Transform m_transform;

        /// <summary>
        /// The current hold state of the object (Picked up, Dropped, etc.).
        /// </summary>
        private HoldState holdState;


        #region Interaction Setup fields

        public override bool UsePrimaryActionHint() => holdState == HoldState.Dropped;
        public override string GetPrimaryHintText() => "Pick up";

        public override bool UseSecondaryActionHint() => holdState == HoldState.Holding;
        public override string GetSecondaryHintText() => "Drop";

        public override bool IsUnsubscribeBlocked() => holdState == HoldState.Picking || holdState == HoldState.Holding;

        #endregion


        #region Interaction System

        public override void OnInteractPrimary()
        {
            if (holdState != HoldState.Dropped)
                return;

            PickUp();
        }

        public override void OnInteractSecondary()
        {
            if (holdState == HoldState.Holding)
            {
                Drop();
            }
        }

        /// <summary>
        /// Subscribes other interactables to events when picked up.
        /// </summary>
        private IEnumerator SubscribeOtherInteractions()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            interactables.ForEach(interactable =>
            {
                interactable.enabled = true;
                interactable.Subscribe();
            });
        }

        /// <summary>
        /// Unsubscribes other interactables from events when dropped.
        /// </summary>
        private IEnumerator UnsubscribeOtherInteractions()
        {
            yield return new WaitForEndOfFrame();
            interactables.ForEach(interactable =>
            {
                interactable.enabled = false;
                interactable.Unsubscribe();
            });
        }

        #endregion


        private void Start()
        {
            m_transform = transform;
        }


        /// <summary>
        /// Handles the actual pickup of the object, including sound, state updates,
        /// player restrictions, component disabling, subscribing to other interactions,
        /// and moving the object to the holding point.
        /// </summary>
        private void PickUp()
        {
            if (useSound)
                SoundPlayer.Instance.PlayOneShot(transform.position, soundItem);

            UpdateState(HoldState.Picking);
            PlayerRestrictions.AddState(PlayerRestrictions.RestrictionState.IsHoldingItem);

            SetComponentsEnabled(false);
            UpdateState(HoldState.Holding);
            StartCoroutine(SubscribeOtherInteractions());
            MoveToHoldingPoint();
        }

        /// <summary>
        /// Handles the dropping of the object, including state updates, component enabling,
        /// unsubscribing from other interactions, removing player restrictions, detaching
        /// the object, and unsubscribing from interaction events.
        /// </summary>
        private void Drop()
        {
            UpdateState(HoldState.Dropping);

            SetComponentsEnabled(true);
            StartCoroutine(UnsubscribeOtherInteractions());
            PlayerRestrictions.RemoveState(PlayerRestrictions.RestrictionState.IsHoldingItem);
            UpdateState(HoldState.Dropped);
            transform.Detach();
            Unsubscribe();
        }

        /// <summary>
        /// Moves the object to its appropriate holding position relative to the player
        /// or a specified parent object.
        /// </summary>
        private void MoveToHoldingPoint()
        {
            Transform transformOrigin = InteractionRaycaster.Instance.GetTransformOrigin();
            Vector3 raycastOrigin = transformOrigin.position;
            Vector3 forwardDirection = m_transform.position - raycastOrigin;
            Vector3 holdPosition = raycastOrigin + forwardDirection * examineDistance +
                                   transformOrigin.TransformDirection(moveOffset);

            transform.position = holdPosition;
            transform.AttachToParentOrPlayer(attachPoint);
        }


        /// <summary>
        /// Enables or disables components for physics-related behavior, such as gravity
        /// and kinematic movement, based on the specified boolean value.
        /// </summary>
        /// <param name="enabled">True to enable components, false to disable them.</param>
        private void SetComponentsEnabled(bool enabled)
        {
            Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
            if (rigidbody)
            {
                rigidbody.useGravity = enabled;
                rigidbody.isKinematic = !enabled;
            }
        }

        /// <summary>
        /// Updates the current hold state of the object and triggers any necessary UI hints.
        /// </summary>
        /// <param name="newHoldState">The new hold state to be set.</param>
        private void UpdateState(HoldState newHoldState)
        {
            holdState = newHoldState;
            UpdateHints();
        }
    }
}