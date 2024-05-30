using System.Collections.Generic;
using System.Linq;
using EIS.Runtime.Core.Helpers;
using EIS.Runtime.Misc;
using UnityEngine;

namespace EIS.Runtime.Core
{
    /// <summary>
    /// Singleton class that handles interaction with interactable objects in the scene.
    /// </summary>
    public class InteractionHandler : Singleton<InteractionHandler>
    {
        /// <summary>
        /// Name of the button used for primary interaction (e.g., "InteractPrimary").
        /// </summary>
        private const string primaryInteractionButton = "InteractPrimary";

        /// <summary>
        /// Name of the button used for secondary interaction (e.g., "InteractSecondary").
        /// </summary>
        private const string secondaryInteractionButton = "InteractSecondary";

        /// <summary>
        /// Set of currently subscribed interactable objects.
        /// </summary>
        private HashSet<Interactable> runningInteractables = new HashSet<Interactable>();

        /// <summary>
        /// Subscribes an interactable object to receive interaction events.
        /// </summary>
        /// <param name="interactable">The interactable object to subscribe.</param>
        public void Subscribe(Interactable interactable)
        {
            interactable = CheckForProxy(interactable);
            runningInteractables.Add(interactable);
            interactable.isSubscribed = true;
        }

        /// <summary>
        /// Unsubscribes an interactable object from receiving interaction events.
        /// </summary>
        /// <param name="interactable">The interactable object to unsubscribe.</param>
        public void Unsubscribe(Interactable interactable)
        {
            interactable = CheckForProxy(interactable);
            runningInteractables.Remove(interactable);
            interactable.isSubscribed = false;
        }

        /// <summary>
        /// Checks if the provided interactable is actually a proxy object, and if so, returns the actual interactable it represents.
        /// </summary>
        /// <param name="interactable">The interactable object to check.</param>
        /// <returns>The actual interactable object if it's a proxy, otherwise the original interactable.</returns>
        private Interactable CheckForProxy(Interactable interactable)
        {
            return interactable.GetType() == typeof(InteractableProxy)
                ? ((InteractableProxy)interactable).iinteractable
                : interactable;
        }

        private void Update()
        {
            FilterNullInteractions();
            if (Input.GetButtonDown(primaryInteractionButton))
            {
                for (int i = 0; i < runningInteractables.Count; i++)
                {
                    runningInteractables.ElementAt(i).OnInteractPrimary();
                }
            }
            else if (Input.GetButtonDown(secondaryInteractionButton))
            {
                for (int i = 0; i < runningInteractables.Count; i++)
                {
                    runningInteractables.ElementAt(i).OnInteractSecondary();
                }
            }

            //Tick
            for (int i = 0; i < runningInteractables.Count; i++)
            {
                runningInteractables.ElementAt(i).Tick();
            }

            if (Debug.isDebugBuild)
            {
                DebugInteractions();
            }
        }

        /// <summary>
        /// Removes any null references from the runningInteractables set.
        /// </summary>
        private void FilterNullInteractions()
        {
            for (int i = 0; i < runningInteractables.Count; i++)
            {
                Interactable elementAt = runningInteractables.ElementAt(i);
                if (elementAt == null)
                {
                    runningInteractables.Remove(elementAt);
                }
            }
        }

        private void LateUpdate()
        {
            //Late Tick
            for (int i = 0; i < runningInteractables.Count; i++)
            {
                runningInteractables.ElementAt(i).LateTick();
            }
        }

        private void FixedUpdate()
        {
            //Fixed Tick
            for (int i = 0; i < runningInteractables.Count; i++)
            {
                runningInteractables.ElementAt(i).FixedTick();
            }
        }

        /// <summary>
        /// Logs information about running interactable objects to the console when the "L" key is pressed.
        /// </summary>
        private void DebugInteractions()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                foreach (Interactable runningInteractable in runningInteractables)
                {
                    Debug.Log(runningInteractable.ToString());
                }
            }
        }
    }
}