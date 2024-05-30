using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EIS.Runtime.Controls;
using EIS.Runtime.Core;
using EIS.Runtime.Core.Hints;
using EIS.Runtime.Extensions;
using EIS.Runtime.Restrictions;
using EIS.Runtime.States;
using UnityEngine;

namespace EIS.Runtime.Interactions.ItemInteraction
{
    /// <summary>
    /// An interactable component that enables a player to examine an object closely, 
    /// rotating and zooming in on it.
    /// </summary>
    public class ExaminationInteraction : Interactable
    {
        /// <summary>
        /// The speed at which the object rotates when examined.
        /// </summary>
        [Tooltip("The speed at which the object rotates when examined")] [SerializeField]
        private float rotationSpeed = 0.5f;

        /// <summary>
        /// The distance to move the object from the player when starting examination.
        /// </summary>
        [Tooltip("The distance to move the object from the player when starting examination")] [SerializeField]
        private float examineDistance = 0.3f;

        /// <summary>
        /// The speed at which the object moves during examination transitions.
        /// </summary>
        [Tooltip("The speed at which the object moves during examination transitions")] [SerializeField]
        private float moveSpeed = 10;

        /// <summary>
        /// The minimum distance to zoom in on the object during examination.
        /// </summary>
        [Tooltip("The minimum distance to zoom in on the object during examination")] [SerializeField] [Space]
        private float minZoomDistance = -0.5f;

        /// <summary>
        /// The maximum distance to zoom in on the object during examination.
        /// </summary>
        [Tooltip("The maximum distance to zoom in on the object during examination")] [SerializeField]
        private float maxZoomDistance = 0.4f;

        /// <summary>
        /// A list of additional interactables to enable during examination.
        /// </summary>
        [SerializeField] [Tooltip("A list of additional interactables to enable during examination")]
        private List<Interactable> interactables = new List<Interactable>();

        /// <summary>
        /// The Transform of the object being examined.
        /// </summary>
        private Transform m_transform;

        /// <summary>
        /// The original position of the object before examination.
        /// </summary>
        private Vector3 initialPosition;

        /// <summary>
        /// The original rotation of the object before examination.
        /// </summary>
        private Quaternion initialRotation;

        /// <summary>
        /// The target position for the object during examination.
        /// </summary>
        private Vector3 examinationPosition;

        /// <summary>
        /// The last recorded mouse position for rotation input.
        /// </summary>
        private Vector2 lastMousePosition = Vector2.zero;

        /// <summary>
        /// The current zoom level for the object during examination.
        /// </summary>
        private float currentZoom = 0;

        /// <summary>
        /// The current state of the examination process.
        /// </summary>
        private ExamineState examineState;


        #region Interaction Setup fields

        public override bool UsePrimaryActionHint() => false;
        public override string GetPrimaryHintText() => default;

        public override bool UseSecondaryActionHint() =>
            examineState == ExamineState.None || examineState == ExamineState.Examination;

        public override string GetSecondaryHintText() => examineState == ExamineState.None ? "Examine" : "Put down";

        protected override List<InteractionHintData> GetInputHints()
        {
            if (examineState != ExamineState.Examination)
                return default;

            return new List<InteractionHintData>()
            {
                new InteractionHintData(HintType.ZOOM, "Zoom"),
                new InteractionHintData(HintType.ROTATE, "Rotate"),
            };
        }

        public override bool IsUnsubscribeBlocked() => examineState != ExamineState.None;

        #endregion


        #region Interaction System

        public override void OnInteractSecondary()
        {
            if (examineState == ExamineState.Examination)
            {
                StartCoroutine(CancelExamination());
                return;
            }

            if (examineState != ExamineState.None)
                return;

            StartCoroutine(InitializeExamination());
        }

        /// <summary>
        /// Enables additional interactions specified in the interactables list.
        /// </summary>
        private void SubscribeOtherInteractions()
        {
            interactables.ForEach(interactable =>
            {
                interactable.enabled = true;
                interactable.Subscribe();
            });
        }

        /// <summary>
        /// Disables additional interactions previously enabled during examination.
        /// </summary>
        private void UnsubscribeOtherInteractions()
        {
            interactables.ForEach(interactable =>
            {
                interactable.enabled = false;
                interactable.Unsubscribe();
            });
        }

        #endregion

        #region UNITY

        private void Start()
        {
            m_transform = transform;
        }

        private void Update()
        {
            if (examineState == ExamineState.Examination)
            {
                RotateItem();
                ZoomItem();
            }
        }

        #endregion

        /// <summary>
        /// Initializes the examination interaction.
        /// </summary>
        private IEnumerator InitializeExamination()
        {
            UpdateState(ExamineState.InitializeExamination);
            PlayerRestrictions.AddState(PlayerRestrictions.RestrictionState.IsExamining);
            initialPosition = m_transform.position;
            initialRotation = m_transform.rotation;

            SetComponentsEnabled(false);
            yield return StartCoroutine(MoveToExaminationPoint());
            UpdateState(ExamineState.Examination);
            SubscribeOtherInteractions();
        }

        /// <summary>
        /// Cancels the examination interaction.
        /// </summary>
        private IEnumerator CancelExamination()
        {
            UpdateState(ExamineState.CancelExamination);
            yield return StartCoroutine(transform.RotateToQuaternion(initialRotation, moveSpeed));
            yield return StartCoroutine(transform.MoveToPosition(initialPosition, moveSpeed));

            SetComponentsEnabled(true);
            UnsubscribeOtherInteractions();
            PlayerRestrictions.RemoveState(PlayerRestrictions.RestrictionState.IsExamining);
            UpdateState(ExamineState.None);
        }

        /// <summary>
        /// Moves the GameObject to the examination position.
        /// </summary>
        private IEnumerator MoveToExaminationPoint()
        {
            Vector3 raycastOrigin = InteractionRaycaster.Instance.GetRaycastOrigin();
            Vector3 forwardDirection = m_transform.position - raycastOrigin;
            examinationPosition = raycastOrigin + forwardDirection * examineDistance;

            yield return StartCoroutine(transform.MoveToPosition(examinationPosition, moveSpeed));
        }

        /// <summary>
        /// Rotates the GameObject based on mouse input.
        /// </summary>
        private void RotateItem()
        {
            if (lastMousePosition == Vector2.zero)
                lastMousePosition = InputHandler.Instance.GetRotationInput();

            Vector2 delta = InputHandler.Instance.GetRotationInput() - lastMousePosition;
            transform.Rotate(Vector3.up, delta.x * rotationSpeed, Space.Self);
            transform.Rotate(Vector3.right, -delta.y * rotationSpeed, Space.World);
            lastMousePosition = InputHandler.Instance.GetRotationInput();
        }

        /// <summary>
        /// Zooms the examination view based on input.
        /// </summary>
        private void ZoomItem()
        {
            float zoomInput = InputHandler.Instance.GetZoomInput() * 2;
            currentZoom += zoomInput;
            currentZoom = Mathf.Clamp(currentZoom, minZoomDistance, maxZoomDistance);

            Vector3 raycastOrigin = InteractionRaycaster.Instance.GetRaycastOrigin();
            Vector3 forwardDirection = m_transform.position - raycastOrigin;

            Vector3 zoomExaminationPosition = examinationPosition + forwardDirection * currentZoom;
            transform.MoveTo(zoomExaminationPosition, moveSpeed);
        }

        /// <summary>
        /// Enables or disables components on the GameObject.
        /// </summary>
        /// <param name="enabled">Whether to enable or disable the components.</param>
        private void SetComponentsEnabled(bool enabled)
        {
            Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
            if (rigidbody)
            {
                rigidbody.useGravity = enabled;
                rigidbody.isKinematic = !enabled;
            }

            Collider[] colliders = GetComponents<Collider>();
            colliders
                .ToList()
                .ForEach(coll => coll.enabled = enabled);
        }

        /// <summary>
        /// Updates the examination state.
        /// </summary>
        /// <param name="newExamineState">The new examination state.</param>
        private void UpdateState(ExamineState newExamineState)
        {
            examineState = newExamineState;
            UpdateHints();
        }
    }
}