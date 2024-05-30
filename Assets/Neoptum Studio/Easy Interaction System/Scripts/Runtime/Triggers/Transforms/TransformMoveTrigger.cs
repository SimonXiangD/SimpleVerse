using System.Collections.Generic;
using System.Linq;
using EIS.Runtime.Interactions.Transforms;
using EIS.Runtime.States;
using UnityEngine;

namespace EIS.Runtime.Triggers.Transforms
{
    /// <summary>
    /// Triggers transform movement interactions based on object collisions.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TransformMoveTrigger : MonoBehaviour
    {
        /// <summary>
        /// List of transform move interactions to trigger when collisions occur.
        /// </summary>
        [SerializeField]
        protected List<ReversibleTransformInteractable> transformMoveInteractions =
            new List<ReversibleTransformInteractable>();

        /// <summary>
        /// Checks for trigger components in Unity Editor and logs a warning if none are found.
        /// </summary>
        private void Start()
        {
#if UNITY_EDITOR
            Collider[] colliders = GetComponents<Collider>();
            bool any = colliders.Any(coll => coll.isTrigger);
            if (!any)
            {
                Debug.LogWarning("Please add at least one trigger on this object");
            }
#endif
        }

        /// <summary>
        /// Triggers the forward movement of associated transform move interactions when a collider enters.
        /// </summary>
        /// <param name="other">The collider that entered the trigger.</param>
        private void OnTriggerEnter(Collider other)
        {
            transformMoveInteractions
                .ForEach(trsMoveInter => trsMoveInter.ChangeStateTo(ReversibleProgressionState.StartedForward));
        }

        /// <summary>
        /// Triggers the backward movement of associated transform move interactions when a collider exits.
        /// </summary>
        /// <param name="other">The collider that exited the trigger.</param>
        private void OnTriggerExit(Collider other)
        {
            transformMoveInteractions
                .ForEach(trsMoveInter => trsMoveInter.ChangeStateTo(ReversibleProgressionState.StartedBackward));
        }
    }
}