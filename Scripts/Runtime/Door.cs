using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class Door : MonoBehaviour
    {
        [SerializeField]
        private Cell _cell;
        public Cell Cell { get => _cell; set => _cell = value; }

        [SerializeField]
        private DoorDirection _direction;
        public DoorDirection Direction { get => _direction; set => _direction = value; }

        [SerializeField]
        private DoorType _type;
        public DoorType Type { get => _type; set => _type = value; }

        [SerializeField]
        private int _code;
        public int Code { get => _code; set => _code = value; }
    }
}
