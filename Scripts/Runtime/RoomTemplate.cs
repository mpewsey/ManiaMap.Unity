using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class RoomTemplate : MonoBehaviour
    {
        [field: SerializeField]
        public int Id { get; set; }

        [field: SerializeField]
        public string Name { get; set; } = string.Empty;

        [field: SerializeField]
        public Vector2Int Size { get; set; }
    }
}