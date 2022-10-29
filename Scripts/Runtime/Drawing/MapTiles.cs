using System.Collections.Generic;
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

        [Header("Optional")]

        [SerializeField]
        private Texture2D _grid;
        /// <summary>
        /// The tile used for the LayoutMap grid (optional).
        /// </summary>
        public Texture2D Grid { get => _grid; set => _grid = value; }

        [SerializeField]
        private Texture2D _savePoint;
        /// <summary>
        /// The tile used for save point features (optional).
        /// </summary>
        public Texture2D SavePoint { get => _grid; set => _grid = value; }

        /// <summary>
        /// A dictionary of referenced tiles.
        /// </summary>
        private Dictionary<string, Texture2D> TileDictionary { get; } = new Dictionary<string, Texture2D>();

        private void OnEnable()
        {
            CreateTileDictionary();
        }

        public void CreateTileDictionary()
        {
            TileDictionary.Clear();
            TileDictionary.EnsureCapacity(12);
            TileDictionary.Add(MapTileType.NorthDoor, NorthDoor);
            TileDictionary.Add(MapTileType.SouthDoor, SouthDoor);
            TileDictionary.Add(MapTileType.EastDoor, EastDoor);
            TileDictionary.Add(MapTileType.WestDoor, WestDoor);
            TileDictionary.Add(MapTileType.TopDoor, TopDoor);
            TileDictionary.Add(MapTileType.BottomDoor, BottomDoor);
            TileDictionary.Add(MapTileType.NorthWall, NorthWall);
            TileDictionary.Add(MapTileType.SouthWall, SouthWall);
            TileDictionary.Add(MapTileType.EastWall, EastWall);
            TileDictionary.Add(MapTileType.WestWall, WestWall);
            TileDictionary.Add(MapTileType.Grid, Grid);
            TileDictionary.Add(MapTileType.SavePoint, SavePoint);
        }

        public Texture2D GetTile(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            if (TileDictionary.TryGetValue(name, out Texture2D tile))
                return tile;
            return null;
        }
    }
}