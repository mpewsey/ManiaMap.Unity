using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class Cell : MonoBehaviour
    {
        [SerializeField]
        private Vector2Int _position;
        public Vector2Int Position { get => _position; set => _position = value; }
    }
}