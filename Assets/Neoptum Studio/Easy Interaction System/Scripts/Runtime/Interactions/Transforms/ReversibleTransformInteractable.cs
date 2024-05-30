using EIS.Runtime.Core;
using EIS.Runtime.Extensions;
using EIS.Runtime.Sound;
using EIS.Runtime.States;
using UnityEngine;

namespace EIS.Runtime.Interactions.Transforms
{
    /// <summary>
    /// An interactable component that allows reversible transformation (position, rotation, scale) of a GameObject.
    /// </summary>
    public class ReversibleTransformInteractable : Interactable
    {
        /// <summary>
        /// The text hint to display for interaction.
        /// </summary>
        [SerializeField] private string hint;

        /// <summary>
        /// Determines whether the interaction requires changing the position of the GameObject.
        /// </summary>
        [SerializeField] public bool requireChangePosition = false;

        /// <summary>
        /// Determines whether the interaction requires changing the rotation of the GameObject.
        /// </summary>
        [SerializeField] public bool requireChangeRotation = false;

        /// <summary>
        /// Determines whether the interaction requires changing the scale of the GameObject.
        /// </summary>
        [SerializeField] public bool requireChangeScale = false;


        /// <summary>
        /// The new position to be applied upon interaction (hidden in Inspector).
        /// </summary>
        [HideInInspector] public Vector3 newPosition;

        /// <summary>
        /// The new rotation to be applied upon interaction (hidden in Inspector).
        /// </summary>
        [HideInInspector] public Quaternion newRotation;

        /// <summary>
        /// The new scale to be applied upon interaction (hidden in Inspector).
        /// </summary>
        [HideInInspector] public Vector3 newScale;

        /// <summary>
        /// The minimum distance threshold for considering the position change complete.
        /// </summary>
        [HideInInspector] public float thresholdPosition = 0.01f;

        /// <summary>
        /// The minimum angle threshold (in degrees) for considering the rotation change complete.
        /// </summary>
        [HideInInspector] public float thresholdRotation = 2f;

        /// <summary>
        /// The minimum distance threshold for considering the scale change complete.
        /// </summary>
        [HideInInspector] public float thresholdScale = 0.01f;

        /// <summary>
        /// The speed at which the transformation occurs during interaction.
        /// </summary>
        [SerializeField] private float changeSpeed = 1f;

        /// <summary>
        /// Determines whether to play a sound effect when the interaction is triggered.
        /// </summary>
        [Space] [SerializeField] private bool useSound = false;

        /// <summary>
        /// The SoundItem to play when the interaction is triggered, if useSound is true.
        /// </summary>
        [SerializeField] private SoundItem soundItem;

        /// <summary>
        /// Stores the initial position of the GameObject.
        /// </summary>
        private Vector3 initialPosition;

        /// <summary>
        /// Stores the initial rotation of the GameObject.
        /// </summary>
        private Quaternion initialRotation;

        /// <summary>
        /// Stores the initial scale of the GameObject.
        /// </summary>
        private Vector3 initialScale;


        /// <summary>
        /// A reference to the transform component of the GameObject.
        /// </summary>
        private Transform mTransform;

        /// <summary>
        /// The current state of the reversible transformation progress.
        /// </summary>
        private ReversibleProgressionState progressionState = ReversibleProgressionState.NotStarted;


        #region Interaction Setup fields

        public override bool UsePrimaryActionHint() => !GetPrimaryHintText().IsNullOrEmpty();
        public override string GetPrimaryHintText() => GetPrimaryHint();

        public override bool UseSecondaryActionHint() => false;

        public override string GetSecondaryHintText() => default;

        private string GetPrimaryHint()
        {
            if (hint.IsNullOrEmpty())
            {
                return default;
            }

            return progressionState switch
            {
                ReversibleProgressionState.NotStarted => "Interact",
                ReversibleProgressionState.StartedForward => default,
                ReversibleProgressionState.FinishedForward => "Interact",
                ReversibleProgressionState.StartedBackward => default,
                _ => default
            };
        }

        public override bool IsUnsubscribeBlocked() => false;

        #endregion


        private void Start()
        {
            mTransform = transform;

            initialPosition = mTransform.position;
            initialRotation = mTransform.rotation;
            initialScale = mTransform.localScale;

            ChangeStateTo(ReversibleProgressionState.NotStarted);
        }

        public override void OnInteractPrimary()
        {
            if (progressionState == ReversibleProgressionState.StartedForward
                || progressionState ==  ReversibleProgressionState.StartedBackward)
            {
                return;
            }

            if (progressionState == ReversibleProgressionState.NotStarted)
            {
                ChangeStateTo(ReversibleProgressionState.StartedForward);

                if (useSound)
                    SoundPlayer.Instance.PlayOneShot(transform.position, soundItem);
            }

            if (progressionState == ReversibleProgressionState.FinishedForward)
            {
                ChangeStateTo(ReversibleProgressionState.StartedBackward);

                if (useSound)
                    SoundPlayer.Instance.PlayOneShot(transform.position, soundItem);
            }
        }

        private void Update()
        {
            if (progressionState == ReversibleProgressionState.StartedForward || 
                progressionState == ReversibleProgressionState.StartedBackward)
            {
                ExecuteMove();
            }
        }

        /// <summary>
        /// Performs the actual transformation update for each frame during the interaction.
        /// </summary>
        private void ExecuteMove()
        {
            Vector3 selectedPosition = initialPosition;
            Quaternion selectedRotation = initialRotation;
            Vector3 selectedScale = initialScale;

            if (progressionState == ReversibleProgressionState.StartedForward)
            {
                selectedPosition = newPosition;
                selectedRotation = newRotation;
                selectedScale = newScale;
            }

            bool areAllFinished = true;

            if (requireChangePosition)
                areAllFinished = ChangePosition(selectedPosition);

            if (requireChangeRotation)
                areAllFinished = areAllFinished && ChangeRotation(selectedRotation);

            if (requireChangeScale)
                areAllFinished = areAllFinished && ChangeScale(selectedScale);

            if (areAllFinished)
            {
                ChangeNextState();
            }
        }

        /// <summary>
        /// Attempts to change the position of the GameObject, returning true if completed.
        /// </summary>
        private bool ChangePosition(Vector3 selectedPosition)
        {
            Vector3 direction = selectedPosition - mTransform.position;

            direction.Normalize();

            mTransform.position += direction * (changeSpeed * Time.deltaTime);

            if (Vector3.Distance(mTransform.position, selectedPosition) < thresholdPosition)
            {
                mTransform.position = selectedPosition;
                return true;
            }

            return false;
        }


        /// <summary>
        /// Attempts to change the rotation of the GameObject, returning true if completed.
        /// </summary>
        private bool ChangeRotation(Quaternion selectedRotation)
        {
            // Calculate the rotation in shortest direction (spherical interpolation)
            Quaternion difference = Quaternion.Inverse(mTransform.rotation) * selectedRotation;

            // Get the angle between rotations (in radians)
            float angle = Mathf.Min(180f, Quaternion.Angle(mTransform.rotation, selectedRotation));
            float maxRotationPerFrame = changeSpeed * 10 * Time.deltaTime;

            // Rotate by the minimum of angle and max rotation per frame
            Quaternion rotationStep = Quaternion.Slerp(Quaternion.identity, difference,
                Mathf.Min(angle, maxRotationPerFrame) / angle);

            mTransform.rotation *= rotationStep;

            // Check if reached the threshold angle
            if (Quaternion.Angle(mTransform.rotation, selectedRotation) < thresholdRotation)
            {
                mTransform.rotation = selectedRotation;
                return true;
            }

            return false;
        }


        /// <summary>
        /// Attempts to change the scale of the GameObject, returning true if completed.
        /// </summary>
        private bool ChangeScale(Vector3 selectedScale)
        {
            // Calculate the difference in scale
            Vector3 scaleDifference = selectedScale - mTransform.localScale;

            // Get the magnitude of the difference (distance)
            float differenceMagnitude = Vector3.Magnitude(scaleDifference);
            float maxScaleChangePerFrame = changeSpeed * Time.deltaTime;

            // Scale by the minimum of difference magnitude and max scale change
            Vector3 scaleStep = mTransform.localScale +
                                Mathf.Min(differenceMagnitude, maxScaleChangePerFrame) * scaleDifference.normalized;

            mTransform.localScale = scaleStep;

            // Check if reached the threshold distance
            if (Vector3.Distance(mTransform.localScale, selectedScale) < thresholdScale)
            {
                mTransform.localScale = selectedScale;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Advances the reversible progression to the appropriate next state.
        /// </summary>
        private void ChangeNextState()
        {
            if (progressionState == ReversibleProgressionState.StartedForward)
            {
                ChangeStateTo(ReversibleProgressionState.FinishedForward);
            }

            else if (progressionState == ReversibleProgressionState.StartedBackward)
            {
                ChangeStateTo(ReversibleProgressionState.NotStarted);
            }
        }

        /// <summary>
        /// Updates the internal progression state and any related UI elements.
        /// </summary>
        public void ChangeStateTo(ReversibleProgressionState state)
        {
            progressionState = state;
            UpdateHints();
        }
    }
}