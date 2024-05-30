using UnityEngine;

namespace EIS.Runtime.Player
{
    /// <summary>
    /// Prevents collisions between the player's character controller and colliders on this GameObject.
    /// </summary>
    public class PlayerCollisionIgnorer : MonoBehaviour
    {
        /// <summary>
        /// Sets up collision ignoring between this GameObject's colliders and the player's character controller.
        /// </summary>
        private void Start()
        {
            // Get all colliders attached to this GameObject.
            Collider[] colliders = GetComponents<Collider>();

            // Get the player's character controller.
            Collider characterController = PlayerController.Instance.GetComponent<CharacterController>();

            // For each collider on this GameObject, ignore collisions with the player's character controller.
            foreach (Collider col in colliders)
            {
                Physics.IgnoreCollision(col, characterController);
            }
        }
    }
}