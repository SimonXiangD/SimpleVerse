using System;
using System.Collections.Generic;

namespace EIS.Runtime.Restrictions
{
    /// <summary>
    /// Manages various restrictions that can be applied to the player's actions.
    /// </summary>
    public class PlayerRestrictions
    {
        /// <summary>
        /// Flags enum representing various restriction states that can be applied to the player.
        /// </summary>
        [Flags]
        public enum RestrictionState
        {
            /// <summary>
            /// Indicates the player is currently examining an object.
            /// </summary>
            IsExamining,

            /// <summary>
            /// Indicates the player is currently holding an item.
            /// </summary>
            IsHoldingItem,

            /// <summary>
            /// Indicates the player is currently interacting with a UI element.
            /// </summary>
            IsInUI
        }

        /// <summary>
        /// Internal list to store currently active restriction states.
        /// </summary>
        private static List<RestrictionState> _restrictionStates = new List<RestrictionState>();


        /// <summary>
        /// Adds a new restriction state to the player.
        /// </summary>
        /// <param name="restrictionState">The restriction state to add.</param>
        public static void AddState(RestrictionState restrictionState)
        {
            if (!HasState(restrictionState))
            {
                _restrictionStates.Add(restrictionState);
            }
        }

        /// <summary>
        /// Checks if a specific restriction state is currently active for the player.
        /// </summary>
        /// <param name="restrictionState">The restriction state to check.</param>
        /// <returns>True if the restriction state is active, False otherwise.</returns>
        public static bool HasState(RestrictionState restrictionState)
        {
            return _restrictionStates.Contains(restrictionState);
        }

        /// <summary>
        /// Removes a restriction state from the player.
        /// </summary>
        /// <param name="restrictionState">The restriction state to remove.</param>
        public static void RemoveState(RestrictionState restrictionState)
        {
            _restrictionStates.Remove(restrictionState);
        }

        /// <summary>
        /// Returns a copy of the list containing all currently active restriction states.
        /// </summary>
        /// <returns>A list of RestrictionState flags.</returns>
        public List<RestrictionState> GetAllStates()
        {
            return new List<RestrictionState>(_restrictionStates);
        }


        /// <summary>
        /// Nested class containing methods to check various player action restrictions based on current states.
        /// </summary>
        public static class Conditions
        {
            /// <summary>
            /// Checks if the player is currently allowed to move.
            /// </summary>
            /// <returns>True if the player can move, False otherwise.</returns>
            public static bool CanMove()
            {
                List<RestrictionState> restrictions = new List<RestrictionState>()
                {
                    RestrictionState.IsExamining,
                    RestrictionState.IsInUI,
                };

                return !restrictions.Exists(HasState);
            }

            /// <summary>
            /// Checks if the player is currently allowed to rotate the camera.
            /// </summary>
            /// <returns>True if the player can rotate the camera, False otherwise.</returns>
            public static bool CanRotateCamera()
            {
                List<RestrictionState> restrictionStates = new List<RestrictionState>{
                    RestrictionState.IsExamining,
                    RestrictionState.IsInUI,
                };

                return !restrictionStates.Exists(HasState);
            }

            /// <summary>
            /// Checks if the player is currently allowed to pick up items.
            /// </summary>
            /// <returns>True if the player can pick up items, False otherwise.</returns>
            public static bool CanPickItem()
            {
                List<RestrictionState> restrictionStates = new List<RestrictionState>
                {
                    RestrictionState.IsHoldingItem,
                    RestrictionState.IsInUI
                };

                return !restrictionStates.Exists(HasState);
            }

            /// <summary>
            /// Checks if the player is currently allowed to perform raycasts.
            /// </summary>
            /// <returns>True if the player can perform raycasts, False otherwise.</returns>
            public static bool CanRaycast()
            {
                List<RestrictionState> restrictionStates = new List<RestrictionState>
                {
                    RestrictionState.IsExamining,
                    RestrictionState.IsHoldingItem,
                    RestrictionState.IsInUI
                };

                return !restrictionStates.Exists(HasState);
            }
        }
    }
}