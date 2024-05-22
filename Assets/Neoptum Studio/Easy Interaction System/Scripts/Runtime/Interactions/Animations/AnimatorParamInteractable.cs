using EIS.Runtime.Core;
using UnityEngine;

namespace EIS.Runtime.Interactions.Animations
{
    /// <summary>
    /// An interactable component that triggers animator parameters when interacted with.
    /// </summary>
    public class AnimatorParamInteractable : Interactable
    {
        /// <summary>
        /// Reference to the Animator component to be controlled.
        /// </summary>
        [Tooltip("Reference to the Animator component to be controlled")] [SerializeField]
        private Animator animator;

        /// <summary>
        /// The type of animator parameter to control (Bool or Trigger).
        /// </summary>
        [Tooltip("The type of animator parameter to control (Bool or Trigger)")] [SerializeField]
        private ParamType paramType = ParamType.BOOL;

        /// <summary>
        /// The name of the animator parameter to control.
        /// </summary>
        [Tooltip("The name of the animator parameter to control")]
        public string parameter;

        /// <summary>
        /// Text hint to display when the parameter is true.
        /// </summary>
        [Tooltip("Text hint to display when the parameter is true")]
        public string primaryHintTextIfParameterTrue;

        /// <summary>
        /// Text hint to display when the parameter is false.
        /// </summary>
        [Tooltip("Text hint to display when the parameter is false")]
        public string primaryHintTextIfParameterFalse;

        /// <summary>
        /// Enumeration for supported animator parameter types.
        /// </summary>
        public enum ParamType
        {
            BOOL,
            TRIGGER
        }


        #region Interaction Setup fields

        public override bool UsePrimaryActionHint() => true;

        public override string GetPrimaryHintText()
        {
            if (paramType == ParamType.BOOL)
                return animator.GetBool(parameter) ? primaryHintTextIfParameterTrue : primaryHintTextIfParameterFalse;

            return primaryHintTextIfParameterTrue;
        }

        public override bool UseSecondaryActionHint() => false;
        public override string GetSecondaryHintText() => default;
        public override bool IsUnsubscribeBlocked() => false;

        #endregion


        public override void OnInteractPrimary()
        {
            switch (paramType)
            {
                case ParamType.BOOL:
                    animator.SetBool(parameter, !animator.GetBool(parameter));
                    break;
                case ParamType.TRIGGER:
                    animator.SetTrigger(parameter);
                    break;
            }
        }
    }
}