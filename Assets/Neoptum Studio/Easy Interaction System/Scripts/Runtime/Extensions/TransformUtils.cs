using System.Collections;
using EIS.Runtime.Player;
using UnityEngine;

namespace EIS.Runtime.Extensions
{
    /// <summary>
    /// This static class provides utility methods for working with Transform components.
    /// </summary>
    public static class TransformUtils
    {
        /// <summary>
        /// The threshold used to determine when a movement or rotation is considered complete.
        /// </summary>
        private const float Threshold = 0.2f;


        /// <summary>
        ///  Moves a transform towards a target position over time.
        /// </summary>
        /// <param name="transform">The transform to move.</param>
        /// <param name="position">The target position to move to.</param>
        /// <param name="moveSpeed">The speed at which to move the transform (default: 10).</param>
        public static void MoveTo(this Transform transform, Vector3 position, float moveSpeed = 10)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * moveSpeed);
        }

        /// <summary>
        ///  Moves a transform towards a target position over time as a coroutine.
        /// </summary>
        /// <param name="transform">The transform to move.</param>
        /// <param name="position">The target position to move to.</param>
        /// <param name="moveSpeed">The speed at which to move the transform (default: 10).</param>
        /// <returns>An enumerator that can be used to control the coroutine.</returns>
        public static IEnumerator MoveToPosition(this Transform transform, Vector3 position, float moveSpeed = 10)
        {
            while (Vector3.Distance(transform.position, position) > Threshold)
            {
                transform.MoveTo(position, moveSpeed);
                yield return new WaitForEndOfFrame();
            }

            transform.position = position;
        }

        /// <summary>
        ///  Rotates a transform towards a target rotation over time as a coroutine.
        /// </summary>
        /// <param name="transform">The transform to rotate.</param>
        /// <param name="targetRotation">The target rotation to rotate to.</param>
        /// <param name="rotationSpeed">The speed at which to rotate the transform (default: 10f).</param>
        /// <returns>An enumerator that can be used to control the coroutine.</returns>
        public static IEnumerator RotateToQuaternion(this Transform transform, Quaternion targetRotation,
            float rotationSpeed = 10f)

        {
            while (Quaternion.Angle(transform.rotation, targetRotation) > Threshold)
            {
                // Calculate the step towards the target rotation
                Quaternion step = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // Rotate the transform towards the target
                transform.rotation = step;

                yield return new WaitForEndOfFrame();
            }

            // Set the final rotation precisely
            transform.rotation = targetRotation;
        }

        /// <summary>
        ///  Attaches a transform to a parent transform, or to the player if no parent is specified.
        /// </summary>
        /// <param name="transform">The transform to attach.</param>
        /// <param name="parent">The optional parent transform to attach to.</param>
        public static void AttachToParentOrPlayer(this Transform transform, Transform parent)
        {
            if (parent == null)
            {
                transform.AttachToPlayer();
            }
            else
            {
                transform.parent = parent;
            }
        }

        /// <summary>
        ///  Attaches a transform to the player's transform.
        /// </summary>
        /// <param name="transform">The transform to attach to the player.</param>
        public static void AttachToPlayer(this Transform transform)
        {
            PlayerController controller = Object.FindFirstObjectByType<PlayerController>();
            transform.parent = controller.transform;
        }

        /// <summary>
        ///  Detaches a transform from its parent, making it a root object.
        /// </summary>
        /// <param name="transform">The transform to detach.</param>
        public static void Detach(this Transform transform)
        {
            transform.parent = null;
        }
    }
}