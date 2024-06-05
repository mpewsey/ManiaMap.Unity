using UnityEngine;

namespace MPewsey.ManiaMapUnity.Examples
{
    /// <summary>
    /// Contains events for the collectable spots used in the samples.
    /// </summary>
    [RequireComponent(typeof(CollectableSpotComponent))]
    public class CollectableSpotSample : MonoBehaviour
    {
        /// <summary>
        /// The attached collectable spot component.
        /// </summary>
        public CollectableSpotComponent CollectableSpot { get; private set; }

        private void Awake()
        {
            CollectableSpot = GetComponent<CollectableSpotComponent>();
            CollectableSpot.OnInitialize.AddListener(OnInitialize);
        }

        /// <summary>
        /// The on initialize event. If the collectable spot does not exist,
        /// destroys it.
        /// </summary>
        /// <param name="spot">The collectable spot sending the event.</param>
        private void OnInitialize()
        {
            if (!CollectableSpot.CollectableExists())
                Destroy(CollectableSpot.gameObject);
        }
    }
}
