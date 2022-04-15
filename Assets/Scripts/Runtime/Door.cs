using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class Door : MonoBehaviour
    {
        [field: SerializeField]
        public Cell Cell { get; set; }

        [field: SerializeField]
        public DoorDirection Direction { get; set; }

        [field: SerializeField]
        public DoorType Type { get; set; }

        [field: SerializeField]
        public int Code { get; set; }
    }
}
