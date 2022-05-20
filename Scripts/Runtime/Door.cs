using UnityEngine;
using UnityEngine.Events;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A component representing a door.
    /// </summary>
    public class Door : MonoBehaviour
    {
        [SerializeField]
        private Cell _cell;
        /// <summary>
        /// The parent cell.
        /// </summary>
        public Cell Cell { get => _cell; set => _cell = value; }

        [SerializeField]
        private DoorDirection _direction;
        /// <summary>
        /// The door direction.
        /// </summary>
        public DoorDirection Direction { get => _direction; set => _direction = value; }

        [SerializeField]
        private DoorType _type;
        /// <summary>
        /// The door type.
        /// </summary>
        public DoorType Type { get => _type; set => _type = value; }

        [SerializeField]
        private int _code;
        /// <summary>
        /// The door code.
        /// </summary>
        public int Code { get => _code; set => _code = value; }

        [SerializeField]
        private UnityEvent _onDoorExists = new UnityEvent();
        /// <summary>
        /// The event invoked when a door exists at the location.
        /// </summary>
        public UnityEvent OnDoorExists { get => _onDoorExists; set => _onDoorExists = value; }

        [SerializeField]
        private UnityEvent _onNoDoorExists = new UnityEvent();
        /// <summary>
        /// The event invoked when no door exists at the location.
        /// </summary>
        public UnityEvent OnNoDoorExists { get => _onNoDoorExists; set => _onNoDoorExists = value; }
    }
}
