using UnityEngine;

namespace MPewsey.ManiaMapUnity.Examples
{
    /// <summary>
    /// Contains events for the doors used in the samples.
    /// </summary>
    [RequireComponent(typeof(DoorBehavior))]
    public class SampleDoorEvents : MonoBehaviour
    {
        /// <summary>
        /// The attached door component.
        /// </summary>
        public DoorBehavior Door { get; private set; }

        private void Awake()
        {
            Door = GetComponent<DoorBehavior>();
            Door.OnInitialize.AddListener(OnInitialize);
        }

        /// <summary>
        /// The on initialize event. If the door exists, destroys it, leaving an opening.
        /// </summary>
        /// <param name="door">The door sending the event.</param>
        private void OnInitialize(DoorBehavior door)
        {
            if (door.Exists())
                Destroy(door.gameObject);
        }
    }
}
