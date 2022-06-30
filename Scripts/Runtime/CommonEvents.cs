using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// Contains common events.
    /// </summary>
    public class CommonEvents : MonoBehaviour
    {
        /// <summary>
        /// Destroys the target GameObject.
        /// </summary>
        /// <param name="obj">The GameObject.</param>
        public void DestroyTarget(GameObject obj)
        {
            Destroy(obj);
        }

        /// <summary>
        /// Immediately destroys the target GameObject.
        /// </summary>
        /// <param name="obj">The GameObject.</param>
        public void DestroyImmediateTarget(GameObject obj)
        {
            DestroyImmediate(obj);
        }

        /// <summary>
        /// Destroys the target component.
        /// </summary>
        /// <param name="component">The component.</param>
        public void DestroyComponent(MonoBehaviour component)
        {
            Destroy(component);
        }

        /// <summary>
        /// Immediately destroys the target component.
        /// </summary>
        /// <param name="component">The component.</param>
        public void DestroyImmediateComponent(MonoBehaviour component)
        {
            DestroyImmediate(component);
        }
    }
}
