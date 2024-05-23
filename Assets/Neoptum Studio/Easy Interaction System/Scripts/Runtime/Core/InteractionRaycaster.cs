using System.Linq;
using EIS.Runtime.Extensions;
using EIS.Runtime.Interfaces;
using EIS.Runtime.Misc;
using EIS.Runtime.Restrictions;
using UnityEngine;

namespace EIS.Runtime.Core
{
    /// <summary>
    /// Handles raycasting for interaction with objects in the scene.
    /// </summary>
    public class InteractionRaycaster : Singleton<InteractionRaycaster>
    {
        /// <summary>
        /// The main camera used for raycasting.
        /// </summary>
        private Camera _camera;

        /// <summary>
        /// Layers that the raycast should collide with for interaction.
        /// </summary>
        [Tooltip("Layers that the raycast should collide with for interaction")] [SerializeField]
        private LayerMask layersToCast;

        /// <summary>
        /// Layers that are considered obstacles and block interaction raycasts.
        /// </summary>
        [Tooltip("Layers that are considered obstacles and block interaction raycasts")] [SerializeField]
        private LayerMask obstacles;

        /// <summary>
        /// The currently outlined game object, which is the object the player is hovering over.
        /// </summary>
        private GameObject outlinedGameObject;


        /// <summary>
        /// The width of the screen used for raycast origin calculation.
        /// </summary>
        private float width;

        /// <summary>
        /// The height of the screen used for raycast origin calculation.
        /// </summary>
        private float height;

        private void OnEnable()
        {
            _camera = FindObjectOfType<Camera>();
            width = Screen.width;
            height = Screen.height;
        }

        private void OnDisable()
        {
            OnObjectUnselected();
        }

        private void Update()
        {
            if (!PlayerRestrictions.Conditions.CanRaycast())
            {
                OnObjectUnselected();
                return;
            }

            Ray ray = GetRay();
            bool rayCast = Physics.Raycast(ray, out var hit, 10f, layersToCast | obstacles) &&
                           layersToCast.ContainsLayer(hit.collider.gameObject.layer);

            if (rayCast)
            {
                Debug.DrawRay(ray.origin, ray.direction, Color.red, Time.deltaTime * 10);
            }
            else
            {
#if UNITY_EDITOR
                Debug.DrawRay(ray.origin, ray.direction, Color.green, Time.deltaTime * 10);
                Physics.Raycast(ray, out var hit2, 10f, layersToCast);
#endif
            }


            if (rayCast)
            {
                if (outlinedGameObject != hit.collider.gameObject)
                {
                    OnObjectUnselected();
                    outlinedGameObject = hit.collider.gameObject;
                    OnObjectChanged();
                }
            }
            else
            {
                OnObjectUnselected();
            }
        }

        /// <summary>
        /// Creates a ray based on the main camera and screen center for raycasting.
        /// </summary>
        /// <returns>The generated ray.</returns>
        private Ray GetRay()
        {
            Vector3 screenPoint = new Vector3(width / 2, height / 2, 100);
            Ray ray = _camera.ScreenPointToRay(screenPoint);
            return ray;
        }

        /// <summary>
        /// Called when the player hovers over a new interactable object.
        /// </summary>
        private void OnObjectChanged()
        {
            if (outlinedGameObject == null)
            {
                return;
            }

            EnableOutline();
            EnableTogglable();
            SubscribeInteraction();
        }


        /// <summary>
        /// Called when the player stops hovering over an interactable object.
        /// </summary>
        private void OnObjectUnselected()
        {
            if (outlinedGameObject == null)
            {
                return;
            }

            DisableOutline();
            DisableTogglable();
            UnsubscribeInteraction();
            outlinedGameObject = null;
        }


        /// <summary>
        /// Subscribes the currently outlined object to interaction events if it has any interactable components.
        /// </summary>
        private void SubscribeInteraction()
        {
            Interactable[] interactables = outlinedGameObject.GetComponents<Interactable>();
            if (interactables == null || interactables.Length == 0)
                return;

            interactables
                .Where(interactable => interactable.IsEnabled())
                .ToList()
                .ForEach(interactable => interactable.Subscribe());
        }

        /// <summary>
        /// Unsubscribes from interaction events for all enabled interactables on the outlined game object.
        /// </summary>
        private void UnsubscribeInteraction()
        {
            Interactable[] interactables = outlinedGameObject.GetComponents<Interactable>();
            if (interactables == null || interactables.Length == 0)
                return;

            interactables
                .Where(interactable => interactable.IsEnabled())
                .ToList()
                .ForEach(interactable => interactable.Unsubscribe());
        }


        /// <summary>
        /// Enables the outline component on the outlined game object.
        /// </summary>
        private void EnableOutline()
        {
            QuickOutline.Outline outline = outlinedGameObject.GetComponent<QuickOutline.Outline>();
            if (outline == null)
            {
                return;
            }

            outline.enabled = true;
        }

        /// <summary>
        /// Disables the outline component on the outlined game object.
        /// </summary>
        private void DisableOutline()
        {
            QuickOutline.Outline outline = outlinedGameObject.GetComponent<QuickOutline.Outline>();
            if (outline == null)
            {
                return;
            }

            outline.enabled = false;
        }

        /// <summary>
        /// Enables the ITogglable component on the outlined game object.
        /// </summary>
        private void EnableTogglable()
        {
            ITogglable togglable = outlinedGameObject.GetComponent<ITogglable>();
            if (togglable == null)
            {
                return;
            }

            togglable.Enable();
        }

        /// <summary>
        /// Disables the ITogglable component on the outlined game object.
        /// </summary>
        private void DisableTogglable()
        {
            ITogglable togglable = outlinedGameObject.GetComponent<ITogglable>();
            if (togglable == null)
            {
                return;
            }

            togglable.Disable();
        }

        /// <summary>
        /// Gets the currently outlined game object.
        /// </summary>
        /// <returns>The currently outlined game object, or null if no object is outlined.</returns>
        public static GameObject GetOutlinedGameObject()
        {
            InteractionRaycaster interactionRaycaster = FindObjectOfType<InteractionRaycaster>();
            return interactionRaycaster.outlinedGameObject;
        }

        /// <summary>
        /// Gets the origin of the raycast that outlines the current object.
        /// </summary>
        /// <returns>The origin of the raycast that outlines the current object.</returns>
        public Vector3 GetRaycastOrigin()
        {
            return GetRay().origin;
        }

        /// <summary>
        /// Gets the transform of the camera used for raycasting.
        /// </summary>
        /// <returns>The transform of the camera used for raycasting.</returns>
        public Transform GetTransformOrigin()
        {
            return _camera.transform;
        }
    }
}