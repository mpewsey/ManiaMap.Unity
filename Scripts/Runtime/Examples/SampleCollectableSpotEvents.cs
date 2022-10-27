using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Examples
{
    [RequireComponent(typeof(CollectableSpot))]
    public class SampleCollectableSpotEvents : MonoBehaviour
    {
        public CollectableSpot CollectableSpot { get; private set; }

        private void Awake()
        {
            CollectableSpot = GetComponent<CollectableSpot>();
            CollectableSpot.OnInitialize.AddListener(OnInitialize);
        }

        private void OnInitialize(CollectableSpot spot)
        {
            if (!spot.Exists)
                Destroy(spot.gameObject);
        }
    }
}
