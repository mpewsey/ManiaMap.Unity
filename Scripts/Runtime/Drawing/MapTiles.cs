using System;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    [CreateAssetMenu(menuName = "Mania Map/Map Tiles")]
    public class MapTiles : ScriptableObject
    {
        [SerializeField]
        private Vector2Int _tileSize = new Vector2Int(16, 16);
        public Vector2Int TileSize { get => _tileSize; set => _tileSize = value; }

        [Header("Walls")]

        [SerializeField]
        private Texture2D _northWall;
        public Texture2D NorthWall { get => _northWall; set => _northWall = value; }

        [SerializeField]
        private Texture2D _southWall;
        public Texture2D SouthWall { get => _southWall; set => _southWall = value; }

        [SerializeField]
        private Texture2D _westWall;
        public Texture2D WestWall { get => _westWall; set => _westWall = value; }

        [SerializeField]
        private Texture2D _eastWall;
        public Texture2D EastWall { get => _eastWall; set => _eastWall = value; }

        [Header("Doors")]

        [SerializeField]
        private Texture2D _northDoor;
        public Texture2D NorthDoor { get => _northDoor; set => _northDoor = value; }

        [SerializeField]
        private Texture2D _southDoor;
        public Texture2D SouthDoor { get => _southDoor; set => _southDoor = value; }

        [SerializeField]
        private Texture2D _westDoor;
        public Texture2D WestDoor { get => _westDoor; set => _westDoor = value; }

        [SerializeField]
        private Texture2D _eastDoor;
        public Texture2D EastDoor { get => _eastDoor; set => _eastDoor = value; }

        [SerializeField]
        private Texture2D _topDoor;
        public Texture2D TopDoor { get => _topDoor; set => _topDoor = value; }

        [SerializeField]
        private Texture2D _bottomDoor;
        public Texture2D BottomDoor { get => _bottomDoor; set => _bottomDoor = value; }

        [Header("Grid")]

        [SerializeField]
        private Texture2D _grid;
        public Texture2D Grid { get => _grid; set => _grid = value; }

        public Texture2D GetTile(MapTileType tileType)
        {
            switch (tileType)
            {
                case MapTileType.None:
                    return null;
                case MapTileType.Grid:
                    return Grid;
                case MapTileType.NorthDoor:
                    return NorthDoor;
                case MapTileType.SouthDoor:
                    return SouthDoor;
                case MapTileType.EastDoor:
                    return EastDoor;
                case MapTileType.WestDoor:
                    return WestDoor;
                case MapTileType.TopDoor:
                    return TopDoor;
                case MapTileType.BottomDoor:
                    return BottomDoor;
                case MapTileType.NorthWall:
                    return NorthWall;
                case MapTileType.SouthWall:
                    return SouthWall;
                case MapTileType.EastWall:
                    return EastWall;
                case MapTileType.WestWall:
                    return WestWall;
                default:
                    throw new ArgumentException($"Unhandled tile type: {tileType}.");
            }
        }
    }
}