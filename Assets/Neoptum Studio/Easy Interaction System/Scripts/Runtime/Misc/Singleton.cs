using UnityEngine;

namespace EIS.Runtime.Misc
{
    /// <summary>
    /// Enforces a single instance of a MonoBehaviour type in the scene.
    /// </summary>
    /// <typeparam name="T">Type of the MonoBehaviour to ensure a single instance of.</typeparam>
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        /// <summary>
        /// Stores the singleton instance of the MonoBehaviour.
        /// </summary>
        private static T instance;

        /// <summary>
        /// Gets the singleton instance of the MonoBehaviour.
        /// </summary>
        public static T Instance
        {
            get
            {
                // If the instance hasn't been created yet, find it in the scene.
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();

                    // If the instance is still null, log a warning.
                    if (instance == null)
                    {
                        Debug.LogWarning($"Cannot find instance of {typeof(T).Name}");
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Ensures only one instance of the MonoBehaviour exists in the scene.
        /// </summary>
        protected virtual void Awake()
        {
            // If no instance exists, make this GameObject the instance.
            if (instance == null)
            {
                instance = this as T;
                // DontDestroyOnLoad(this.gameObject);  // If needed for persistence across scenes
            }
            // If another instance exists, destroy this GameObject.
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Resets the singleton instance when this GameObject is destroyed.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}