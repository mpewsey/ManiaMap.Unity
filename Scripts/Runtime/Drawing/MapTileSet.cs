using MPewsey.ManiaMap;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MPewsey.ManiaMapUnity.Drawing
{
    [CreateAssetMenu(menuName = "Mania Map/Map Tile Set")]
    public class MapTileSet : ScriptableObject
    {
        public const int MaxFeatureCount = 64;

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

        [SerializeField]
        private FilterMode _filterMode = FilterMode.Point;
        /// <summary>
        /// The map tile texture filter mode.
        /// </summary>
        public FilterMode FilterMode { get => _filterMode; set => _filterMode = value; }

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

        [Header("Features")]

        [SerializeField]
        private Texture2D _savePoint;
        /// <summary>
        /// The tile used for save point features (optional).
        /// </summary>
        public Texture2D SavePoint { get => _savePoint; set => _savePoint = value; }

        [SerializeField]
        private List<FeatureMapTile> _featureTiles = new List<FeatureMapTile>();
        /// <summary>
        /// A list of cell feature tiles.
        /// </summary>
        public List<FeatureMapTile> FeatureTiles { get => _featureTiles; set => _featureTiles = value; }

        /// <summary>
        /// A dictionary of referenced tiles.
        /// </summary>
        private Dictionary<string, Texture2D> Textures { get; } = new Dictionary<string, Texture2D>();
        private Dictionary<string, long> FeatureFlags { get; } = new Dictionary<string, long>();
        private Dictionary<long, string> FeatureNames { get; } = new Dictionary<long, string>();
        private Dictionary<MapTileKey, Tile> Tiles { get; } = new Dictionary<MapTileKey, Tile>();
        public bool IsDirty { get; private set; } = true;

        private void OnValidate()
        {
            MarkDirty();
        }

        public void MarkDirty()
        {
            IsDirty = true;
        }

        private void PopulateIfDirty()
        {
            if (IsDirty)
            {
                Tiles.Clear();
                PopulateTextures();
                PopulateFeatureFlags();
                IsDirty = false;
            }
        }

        private void PopulateTextures()
        {
            Textures.Clear();
            Textures.Add(MapTileType.NorthDoor, NorthDoor);
            Textures.Add(MapTileType.SouthDoor, SouthDoor);
            Textures.Add(MapTileType.EastDoor, EastDoor);
            Textures.Add(MapTileType.WestDoor, WestDoor);
            Textures.Add(MapTileType.TopDoor, TopDoor);
            Textures.Add(MapTileType.BottomDoor, BottomDoor);
            Textures.Add(MapTileType.NorthWall, NorthWall);
            Textures.Add(MapTileType.SouthWall, SouthWall);
            Textures.Add(MapTileType.EastWall, EastWall);
            Textures.Add(MapTileType.WestWall, WestWall);
            Textures.Add(MapTileType.Grid, Grid);
            Textures.Add(MapTileType.SavePoint, SavePoint);

            foreach (var feature in FeatureTiles)
            {
                Textures.Add(feature.Feature, feature.Tile);
            }
        }

        private void PopulateFeatureFlags()
        {
            FeatureFlags.Clear();
            FeatureNames.Clear();
            AddFeatureFlag(MapTileType.Grid);
            AddFeatureFlag(MapTileType.NorthDoor);
            AddFeatureFlag(MapTileType.SouthDoor);
            AddFeatureFlag(MapTileType.EastDoor);
            AddFeatureFlag(MapTileType.WestDoor);
            AddFeatureFlag(MapTileType.TopDoor);
            AddFeatureFlag(MapTileType.BottomDoor);
            AddFeatureFlag(MapTileType.NorthWall);
            AddFeatureFlag(MapTileType.SouthWall);
            AddFeatureFlag(MapTileType.EastWall);
            AddFeatureFlag(MapTileType.WestWall);
            AddFeatureFlag(MapTileType.SavePoint);

            foreach (var feature in FeatureTiles)
            {
                AddFeatureFlag(feature.Feature);
            }
        }

        private bool AddFeatureFlag(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new System.ArgumentException($"Invalid feature name: {name}.");

            if (FeatureFlags.ContainsKey(name))
                return false;

            if (FeatureFlags.Count >= MaxFeatureCount)
                throw new System.ArgumentException($"Feature count exceeded. Cannot add feature: {name}.");

            var flag = 1 << FeatureFlags.Count;
            FeatureFlags.Add(name, flag);
            FeatureNames.Add(flag, name);
            return true;
        }

        public Texture2D GetTexture(string name)
        {
            PopulateIfDirty();

            if (string.IsNullOrWhiteSpace(name))
                return null;
            if (Textures.TryGetValue(name, out Texture2D tile))
                return tile;
            return null;
        }

        public long GetFeatureFlag(string name)
        {
            PopulateIfDirty();

            if (string.IsNullOrWhiteSpace(name))
                return 0;
            if (FeatureFlags.TryGetValue(name, out long feature))
                return feature;
            return 0;
        }

        public string GetFeatureName(long flag)
        {
            if (FeatureNames.TryGetValue(flag, out string name))
                return name;
            return null;
        }

        public Tile GetTile(long flags, Color32 color)
        {
            var key = new MapTileKey(flags, color);

            if (!Tiles.TryGetValue(key, out Tile tile))
            {
                tile = CreateTile(flags, color);
                Tiles.Add(key, tile);
            }

            return tile;
        }

        private Tile CreateTile(long flags, Color32 color)
        {
            var tile = CreateInstance<Tile>();
            tile.sprite = CreateSprite(CreateFeatureTexture(flags, color));
            return tile;
        }

        private Sprite CreateSprite(Texture2D texture)
        {
            var pivot = new Vector2(0.5f, 0.5f);
            var rect = new Rect(1, 1, texture.width - 2, texture.height - 2);
            return Sprite.Create(texture, rect, pivot, PixelsPerUnit);
        }

        private Texture2D CreateFeatureTexture(long flags, Color32 color)
        {
            var texture = new Texture2D(TileSize.x + 2, TileSize.y + 2);
            texture.filterMode = FilterMode;
            TextureUtility.Fill(texture, color);
            DrawFeatures(texture, flags);
            TextureUtility.FillBorder(texture);
            texture.Apply();
            return texture;
        }

        private void DrawFeatures(Texture2D texture, long flags)
        {
            for (long i = flags; i != 0; i &= i - 1)
            {
                var flag = ~(i - 1) & i;
                var tile = GetTexture(GetFeatureName(flag));
                TextureUtility.DrawImage(texture, tile, Vector2Int.one);
            }
        }
    }
}