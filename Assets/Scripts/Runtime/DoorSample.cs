using UnityEngine;

namespace MPewsey.ManiaMapUnity.Examples
{
    /// <summary>
    /// Contains events for the doors used in the samples.
    /// </summary>
    [RequireComponent(typeof(DoorComponent))]
    public class DoorSample : MonoBehaviour
    {
        [SerializeField] private GameObject _doorContainer;
        public GameObject DoorContainer { get => _doorContainer; set => _doorContainer = value; }

        [SerializeField] private GameObject _wallContainer;
        public GameObject WallContainer { get => _wallContainer; set => _wallContainer = value; }

        /// <summary>
        /// The attached door component.
        /// </summary>
        public DoorComponent Door { get; private set; }

        private void Awake()
        {
            Door = GetComponent<DoorComponent>();
            Door.OnInitialize.AddListener(OnInitialize);
        }

        /// <summary>
        /// The on initialize event. If the door exists, destroys it, leaving an opening.
        /// </summary>
        /// <param name="door">The door sending the event.</param>
        private void OnInitialize()
        {
            var container = Door.DoorExists() ? WallContainer : DoorContainer;
            Destroy(container);
        }
    }
}
