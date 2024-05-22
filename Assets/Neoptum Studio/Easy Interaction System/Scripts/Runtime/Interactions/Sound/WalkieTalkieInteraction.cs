using System.Collections.Generic;
using EIS.Runtime.Controls;
using EIS.Runtime.Core;
using EIS.Runtime.Core.Hints;
using EIS.Runtime.States;
using UnityEngine;

namespace EIS.Runtime.Interactions.Sound
{
    /// <summary>
    /// An interactable component that simulates a walkie-talkie with channel changing and white noise.
    /// </summary>
    public class WalkieTalkieInteraction : Interactable
    {
        /// <summary>
        /// The AudioSource component used to play sounds.
        /// </summary>
        [Tooltip("The AudioSource component used to play sounds")] [SerializeField]
        private AudioSource audioSource;

        /// <summary>
        /// The AudioClip for the white noise played while the walkie-talkie is on.
        /// </summary>
        [Tooltip("The AudioClip for the white noise played while the walkie-talkie is on")] [Space] [SerializeField]
        private AudioClip whiteNoiseClip;

        /// <summary>
        /// The AudioClip played when attempting to change channels.
        /// </summary>
        [Tooltip("The AudioClip played when attempting to change channels")] [SerializeField]
        private AudioClip channelChangeClip;

        /// <summary>
        /// The AudioClip played when successfully switching to the right channel.
        /// </summary>
        [Tooltip("The AudioClip played when successfully switching to the right channel")] [SerializeField]
        private AudioClip rightChannelSound;

        /// <summary>
        /// The duration (in seconds) for the channel changing sound effect.
        /// </summary>
        [Tooltip("The duration (in seconds) for the channel changing sound effect")] [Space] [SerializeField]
        private float channelChangeDuration = 1f;

        /// <summary>
        /// The probability (out of 10) of successfully getting the right channel upon changing.
        /// </summary>
        [Tooltip("The probability (out of 10) of successfully getting the right channel upon changing")]
        [Space]
        [SerializeField]
        [Range(1, 10)]
        private int probabilityToGetRightChannel;

        /// <summary>
        /// The current activity state of the walkie-talkie (On, Off).
        /// </summary>
        private ActivityState activityState = ActivityState.Off;

        /// <summary>
        /// Tracks whether the walkie-talkie is currently attempting to change channels.
        /// </summary>
        private bool isChangingChannel = false;

        /// <summary>
        /// The time when the channel change was initiated (used for duration tracking).
        /// </summary>
        private float channelChangedTime;


        #region Interaction Setup fields

        public override bool UsePrimaryActionHint() => true;
        public override string GetPrimaryHintText() => activityState == ActivityState.Off ? "Turn on" : "Turn off";

        public override bool UseSecondaryActionHint() => false;
        public override string GetSecondaryHintText() => default;

        public override bool IsUnsubscribeBlocked() => default;

        protected override List<InteractionHintData> GetInputHints()
        {
            if (activityState == ActivityState.Off || isChangingChannel)
                return default;

            return new List<InteractionHintData>()
            {
                new InteractionHintData(HintType.CHANGE, "Change channel")
            };
        }

        #endregion


        private void Start()
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.clip = whiteNoiseClip;
        }

        private void Update()
        {
            if (activityState == ActivityState.Off)
                return;

            if (!isChangingChannel && InputHandler.Instance.GetChangeButtonPressDown())
            {
                isChangingChannel = true;
                channelChangedTime = Time.time;

                audioSource.clip = channelChangeClip;
                audioSource.Play();
            }


            if (isChangingChannel)
            {
                if (!(channelChangedTime + channelChangeDuration < Time.time)) return;
                if (Random.Range(0, 10) < probabilityToGetRightChannel)
                {
                    audioSource.clip = rightChannelSound;
                    audioSource.Play();
                }
                else
                {
                    audioSource.clip = whiteNoiseClip;
                    audioSource.Play();
                }

                isChangingChannel = false;
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
        /// Turns on the walkie-talkie, playing the white noise and updating the state.
        /// </summary>
        private void TurnOn()
        {
            ChangeState(ActivityState.On);
            audioSource.Play();
        }

        /// <summary>
        /// Turns off the walkie-talkie, stopping audio and updating the state.
        /// </summary>
        private void TurnOff()
        {
            ChangeState(ActivityState.Off);
            ForceStopChangingChannel();
            audioSource.Stop();
        }

        /// <summary>
        /// Called when the component is disabled, ensuring any ongoing channel changes are stopped.
        /// </summary>
        public override void OnDisable()
        {
            ForceStopChangingChannel();
            base.OnDisable();
        }

        /// <summary>
        /// Forces the walkie-talkie to stop changing channels, returning to white noise.
        /// </summary>
        private void ForceStopChangingChannel()
        {
            if (isChangingChannel)
            {
                isChangingChannel = false;
                audioSource.clip = whiteNoiseClip;
                audioSource.Play();
            }
        }

        /// <summary>
        /// Updates the internal activity state and potentially any related UI elements.
        /// </summary>
        private void ChangeState(ActivityState activityState)
        {
            this.activityState = activityState;
            UpdateHints();
        }
    }
}