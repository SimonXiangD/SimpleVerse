using System.Collections.Generic;
using EIS.Runtime.Controls;
using EIS.Runtime.Core;
using EIS.Runtime.Core.Hints;
using EIS.Runtime.Sound;
using EIS.Runtime.States;
using UnityEngine;

namespace EIS.Runtime.Interactions.Cameras
{
    /// <summary>
    /// An interactable component that controls a camera stream display with optional camera switching and sound effects.
    /// </summary>
    public class CameraStreamDisplayInteraction : Interactable
    {
        /// <summary>
        /// The RenderTexture used to display the camera stream.
        /// </summary>
        [Tooltip("The RenderTexture used to display the camera stream")] [SerializeField]
        private RenderTexture renderTexture;


        /// <summary>
        /// A list of cameras that can be used for the stream display (for switching).
        /// </summary>
        [Tooltip("A list of cameras that can be used for the stream display (for switching).")] [SerializeField]
        private List<Camera> cameras = new List<Camera>();


        /// <summary>
        /// Determines whether to play a sound effect when changing cameras.
        /// </summary>
        [Tooltip("Determines whether to play a sound effect when changing cameras")] [Space] [SerializeField]
        private bool useChannelChangeSound = true;

        /// <summary>
        /// The sound item to play when changing cameras, if useChannelChangeSound is true.
        /// </summary>
        [Tooltip("The sound item to play when changing cameras, if useChannelChangeSound is true")] [SerializeField]
        private SoundItem soundItem;

        /// <summary>
        /// The current activity state of the interaction (On or Off).
        /// </summary>
        private ActivityState activityState = ActivityState.Off;

        /// <summary>
        /// The index of the currently active camera in the cameras list.
        /// </summary>
        private int currentCameraIndex = -1;


        #region Interaction Setup fields

        public override bool UsePrimaryActionHint() => true;
        public override string GetPrimaryHintText() => activityState == ActivityState.Off ? "Turn on" : "Turn off";

        public override bool UseSecondaryActionHint() => false;
        public override string GetSecondaryHintText() => default;

        public override bool IsUnsubscribeBlocked() => false;

        protected override List<InteractionHintData> GetInputHints()
        {
            if (activityState == ActivityState.Off)
                return default;

            return new List<InteractionHintData>()
            {
                new InteractionHintData(HintType.CHANGE, "Change camera")
            };
        }

        #endregion


        private void Start()
        {
            TurnOff();
        }

        public override void Tick()
        {
            if (activityState == ActivityState.On && InputHandler.Instance.GetChangeButtonPressDown())
            {
                ChangeCamera();

                if (useChannelChangeSound)
                    SoundPlayer.Instance.PlayOneShot(transform.position, soundItem);
            }
        }

        public override void OnInteractPrimary()
        {
            if (activityState == ActivityState.Off)
            {
                TurnOn();
            }
            else
            {
                TurnOff();
            }
        }

        /// <summary>
        /// Turns on the stream display and sets the initial camera.
        /// </summary>
        private void TurnOn()
        {
            ChangeCamera();
            ChangeState(ActivityState.On);
        }

        /// <summary>
        /// Turns off the stream display and releases the RenderTexture.
        /// </summary>
        private void TurnOff()
        {
            cameras
                .ForEach(cam => cam.gameObject.SetActive(false));
            renderTexture.Release();
            ChangeState(ActivityState.Off);
        }

        /// <summary>
        /// Changes the active camera displayed on the stream.
        /// </summary>
        private void ChangeCamera()
        {
            if (currentCameraIndex < cameras.Count && currentCameraIndex >= 0)
            {
                cameras[currentCameraIndex].gameObject.SetActive(false);
            }

            if (currentCameraIndex + 1 < cameras.Count)
            {
                currentCameraIndex++;
            }
            else
            {
                currentCameraIndex = 0;
            }

            cameras[currentCameraIndex].gameObject.SetActive(true);
        }

        /// <summary>
        /// Updates the interaction state and potentially interaction hints.
        /// </summary>
        /// <param name="activityState">The new activity state (On or Off).</param>
        private void ChangeState(ActivityState activityState)
        {
            this.activityState = activityState;
            UpdateHints();
        }
    }
}