using UnityEngine.Events;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// An event that passes a CollectableSpot argument.
    /// </summary>
    [System.Serializable]
    public class CollectableSpotEvent : UnityEvent<CollectableSpot>
    {

    }
}