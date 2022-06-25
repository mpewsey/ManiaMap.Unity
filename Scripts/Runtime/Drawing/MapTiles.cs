using System;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    /// <summary>
    /// A grouping of map tiles.
    /// </summary>
    [CreateAssetMenu(menuName = "Mania Map/Map Tiles")]
    public class MapTiles : ScriptableObject
    {
        [SerializeField]
        private int _pixelsPerUnit = 16;
        /// <summary>
        /// The number of pixels per unit for each tile.
        /// </summary>
        public int PixelsPerUnit { get => _pixelsPerUnit; set => _pixelsPerUnit = value; }

        [SerializeField]
        private Vector2Int _tileSize = new Vector2Int(16, 16);
        /// <summary>
        /// The width and height of each tile.
        /// </summary>
        public Vector2Int TileSize { get => _tileSize; set => _tileSize = value; }

        [Header("Walls")]

        [SerializeField]
        private Texture2D _northWall;
        /// <summary>
        /// The superinposed tile when a north wall exists.
        /// </summary>
        public Texture2D NorthWall { get => _northWall; set => _northWall = value; }

        [SerializeField]
        private Texture2D _southWall;
        /// <summary>
        /// The superinposed tile when a south wall exists.
        /// </summary>
        public Texture2D SouthWall { get => _southWall; set => _southWall = value; }

        [SerializeField]
        private Texture2D _westWall;
        /// <summary>
        /// The superimposed tile when a west wall exists.
        /// </summary>
        public Texture2D WestWall { get => _westWall; set => _westWall = value; }

        [SerializeField]
        private Texture2D _eastWall;
        /// <summary>
        /// The superimposed tile when an east wall exists.
        /// </summary>
        public Texture2D EastWall { get => _eastWall; set => _eastWall = value; }

        [Header("Doors")]

        [SerializeField]
        private Texture2D _northDoor;
        /// <summary>
        /// The superimposed tile when a north door exists.
        /// </summary>
        public Texture2D NorthDoor { get => _northDoor; set => _northDoor = value; }

        [SerializeField]
        private Texture2D _southDoor;
        /// <summary>
        /// The superimposed tile when a south door exists.
        /// </summary>
        public Texture2D SouthDoor { get => _southDoor; set => _southDoor = value; }

        [SerializeField]
        private Texture2D _westDoor;
        /// <summary>
        /// The superimposed tile when a west door exists.
        /// </summary>
        public Texture2D WestDoor { get => _westDoor; set => _westDoor = value; }

        [SerializeField]
        private Texture2D _eastDoor;
        /// <summary>
        /// The superimposed tile when an east door exists.
        /// </summary>
        public Texture2D EastDoor { get => _eastDoor; set => _eastDoor = value; }

        [SerializeField]
        private Texture2D _topDoor;
        /// <summary>
        /// The superimposed tile when a top door exists.
        /// </summary>
        public Texture2D TopDoor { get => _topDoor; set => _topDoor = value; }

        [SerializeField]
        private Texture2D _bottomDoor;
        /// <summary>
        /// The superimposed tile when a bottom door exists.
        /// </summary>
        public Texture2D BottomDoor { get => _bottomDoor; set => _bottomDoor = value; }

        [Header("Grid")]

        [SerializeField]
        private Texture2D _grid;
        /// <summary>
        /// The tile used for the LayoutMap grid (optional).
        /// </summary>
        public Texture2D Grid { get => _grid; set => _grid = value; }

        /// <summary>
        /// Returns the tile for the tile type.
        /// </summary>
        /// <param name="tileType">The tile type.</param>
        /// <exception cref="ArgumentException">Raised if the tile type is not handled.</exception>
        public Texture2D GetTile(MapTileTypes tileType)
        {
            switch (tileType)
            {
                case MapTileTypes.None:
                    return null;
                case MapTileTypes.Grid:
                    return Grid;
                case MapTileTypes.NorthDoor:
                    return NorthDoor;
                case MapTileTypes.SouthDoor:
                    return SouthDoor;
                case MapTileTypes.EastDoor:
                    return EastDoor;
                case MapTileTypes.WestDoor:
                    return WestDoor;
                case MapTileTypes.TopDoor:
                    return TopDoor;
                case MapTileTypes.BottomDoor:
                    return BottomDoor;
                case MapTileTypes.NorthWall:
                    return NorthWall;
                case MapTileTypes.SouthWall:
                    return SouthWall;
                case MapTileTypes.EastWall:
                    return EastWall;
                case MapTileTypes.WestWall:
                    return WestWall;
                default:
                    throw new ArgumentException($"Unhandled tile type: {tileType}.");
            }
        }

        /// <summary>
        /// Returns the door tile for the specified direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <exception cref="ArgumentException">Raised if the direction is not handled.</exception>
        public Texture2D GetDoorTile(DoorDirection direction)
        {
            switch (direction)
            {
                case DoorDirection.North:
                    return NorthDoor;
                case DoorDirection.South:
                    return SouthDoor;
                case DoorDirection.East:
                    return EastDoor;
                case DoorDirection.West:
                    return WestDoor;
                case DoorDirection.Top:
                    return TopDoor;
                case DoorDirection.Bottom:
                    return BottomDoor;
                default:
                    throw new ArgumentException($"Unhandled direction: {direction}.");
            }
        }

        /// <summary>
        /// Returns the wall tile for the specified direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <exception cref="ArgumentException">Raised if the direction is not handled.</exception>
        public Texture2D GetWallTile(DoorDirection direction)
        {
            switch (direction)
            {
                case DoorDirection.North:
                    return NorthWall;
                case DoorDirection.South:
                    return SouthWall;
                case DoorDirection.East:
                    return EastWall;
                case DoorDirection.West:
                    return WestWall;
                case DoorDirection.Top:
                case DoorDirection.Bottom:
                    return null;
                default:
                    throw new ArgumentException($"Unhandled direction: {direction}.");
            }
        }

        /// <summary>
        /// Returns the door tile type corresponding to the direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <exception cref="ArgumentException">Raised if the direction is not handled.</exception>
        public static MapTileTypes GetDoorTileType(DoorDirection direction)
        {
            switch (direction)
            {
                case DoorDirection.North:
                    return MapTileTypes.NorthDoor;
                case DoorDirection.South:
                    return MapTileTypes.SouthDoor;
                case DoorDirection.East:
                    return MapTileTypes.EastDoor;
                case DoorDirection.West:
                    return MapTileTypes.WestDoor;
                case DoorDirection.Top:
                    return MapTileTypes.TopDoor;
                case DoorDirection.Bottom:
                    return MapTileTypes.BottomDoor;
                default:
                    throw new ArgumentException($"Unhandled direction: {direction}.");
            }
        }

        /// <summary>
        /// Returns the wall tile type corresponding to the direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <exception cref="ArgumentException">Raised if the direction is not handled.</exception>
        public static MapTileTypes GetWallTileType(DoorDirection direction)
        {
            switch (direction)
            {
                case DoorDirection.North:
                    return MapTileTypes.NorthWall;
                case DoorDirection.South:
                    return MapTileTypes.SouthWall;
                case DoorDirection.East:
                    return MapTileTypes.EastWall;
                case DoorDirection.West:
                    return MapTileTypes.WestWall;
                case DoorDirection.Top:
                case DoorDirection.Bottom:
                    return MapTileTypes.None;
                default:
                    throw new ArgumentException($"Unhandled direction: {direction}.");
            }
        }
    }
}