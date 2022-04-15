using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class Cell : MonoBehaviour
    {
        [field: SerializeField]
        public RoomTemplate Template { get; set; }

        public Vector2Int Position()
        {
            var delta = transform.position - Template.transform.position;
            var x = Mathf.RoundToInt(-delta.y);
            var y = Mathf.RoundToInt(delta.x);
            return new Vector2Int(x, y);
        }
    }
}