using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Examples
{
    /// <summary>
    /// Contains events for the collectable spots used in the samples.
    /// </summary>
    [RequireComponent(typeof(CollectableSpotBehavior))]
    public class SampleCollectableSpotEvents : MonoBehaviour
    {
        /// <summary>
        /// The attached collectable spot component.
        /// </summary>
        public CollectableSpotBehavior CollectableSpot { get; private set; }

        private void Awake()
        {
            CollectableSpot = GetComponent<CollectableSpotBehavior>();
            CollectableSpot.OnInitialize.AddListener(OnInitialize);
        }

        /// <summary>
        /// The on initialize event. If the collectable spot does not exist,
        /// destroys it.
        /// </summary>
        /// <param name="spot">The collectable spot sending the event.</param>
        private void OnInitialize(CollectableSpotBehavior spot)
        {
            if (!spot.Exists())
                Destroy(spot.gameObject);
        }
    }
}
