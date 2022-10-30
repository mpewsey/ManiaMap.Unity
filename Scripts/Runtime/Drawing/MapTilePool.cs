using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    /// <summary>
    /// A pool for creating and caching map tiles.
    /// </summary>
    public class MapTilePool : MonoBehaviour
    {
        /// <summary>
        /// The maximum number of features.
        /// </summary>
        private const int MaxFeatureCount = 64;

        [SerializeField]
        private MapTiles _mapTiles;
        /// <summary>
        /// The map tiles.
        /// </summary>
        public MapTiles MapTiles { get => _mapTiles; set => _mapTiles = value; }

        /// <summary>
        /// A dictionary of feature flags by feature name.
        /// </summary>
        private Dictionary<string, long> FeatureFlags { get; } = new Dictionary<string, long>();

        /// <summary>
        /// A dictionary of feature names by feature flag.
        /// </summary>
        private Dictionary<long, string> FeatureNames { get; } = new Dictionary<long, string>();

        /// <summary>
        /// A dictionary of cached map tiles by hash.
        /// </summary>
        private Dictionary<MapTileHash, Tile> Tiles { get; } = new Dictionary<MapTileHash, Tile>();

        private void Awake()
        {
            AddDefaultFeatures();
        }

        /// <summary>
        /// Adds the default features to the pool.
        /// </summary>
        private void AddDefaultFeatures()
        {
            AddFeature(MapTileType.Grid);
            AddFeature(MapTileType.NorthDoor);
            AddFeature(MapTileType.SouthDoor);
            AddFeature(MapTileType.EastDoor);
            AddFeature(MapTileType.WestDoor);
            AddFeature(MapTileType.TopDoor);
            AddFeature(MapTileType.BottomDoor);
            AddFeature(MapTileType.NorthWall);
            AddFeature(MapTileType.SouthWall);
            AddFeature(MapTileType.EastWall);
            AddFeature(MapTileType.WestWall);
            AddFeature(MapTileType.SavePoint);
        }

        /// <summary>
        /// Clears the pool and resets the default features.
        /// </summary>
        public void Clear()
        {
            Tiles.Clear();
            FeatureFlags.Clear();
            FeatureNames.Clear();
            AddDefaultFeatures();
        }

        /// <summary>
        /// Clears the cached tiles from the pool.
        /// </summary>
        public void ClearTiles()
        {
            Tiles.Clear();
        }

        /// <summary>
        /// Adds the feature to the pool if it doesn't already exist.
        /// </summary>
        /// <param name="name">The feature name.</param>
        /// <exception cref="System.ArgumentException">Raised if the feature name is invalid or the feature count is exceeded.</exception>
        public void AddFeature(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new System.ArgumentException($"Invalid feature name: {name}.");

            if (FeatureFlags.ContainsKey(name))
                return;

            if (FeatureFlags.Count >= MaxFeatureCount)
                throw new System.ArgumentException($"Feature count exceeded. Cannot add feature: {name}.");

            var flag = 1 << FeatureFlags.Count;
            FeatureFlags.Add(name, flag);
            FeatureNames.Add(flag, name);
        }

        /// <summary>
        /// Returns the feature flag corresponding to the name. Returns an empty flag
        /// if the name does not exist or is null or whitespace.
        /// </summary>
        /// <param name="name">The feature name.</param>
        public long GetFeatureFlag(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return 0;
            if (FeatureFlags.TryGetValue(name, out long feature))
                return feature;
            return 0;
        }

        /// <summary>
        /// Returns the feature name corresponding to the feature flag.
        /// </summary>
        /// <param name="flag">The feature flag.</param>
        public string GetFeatureName(long flag)
        {
            if (FeatureNames.TryGetValue(flag, out string name))
                return name;
            return null;
        }

        /// <summary>
        /// Returns the map tile from the pool with the combined features and color.
        /// If the map tile does not already exist in the pool, creates it.
        /// </summary>
        /// <param name="flags">The feature flags.</param>
        /// <param name="color">The tile background color.</param>
        public Tile GetTile(long flags, Color32 color)
        {
            var hash = new MapTileHash(flags, color);

            if (!Tiles.TryGetValue(hash, out Tile tile))
            {
                tile = CreateTile(flags, color);
                Tiles.Add(hash, tile);
            }

            return tile;
        }

        /// <summary>
        /// Returns a new map tile with the specified features.
        /// </summary>
        /// <param name="flags">The feature flags.</param>
        /// <param name="color">The tile background color.</param>
        private Tile CreateTile(long flags, Color32 color)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.name = "Mania Map Tile";
            tile.sprite = CreateSprite(CreateTexture(flags, color));
            return tile;
        }

        /// <summary>
        /// Returns a new sprite for the specified texture.
        /// </summary>
        /// <param name="texture">The sprite texture.</param>
        private Sprite CreateSprite(Texture2D texture)
        {
            var pivot = new Vector2(0.5f, 0.5f);
            var rect = new Rect(1, 1, texture.width - 2, texture.height - 2);
            var sprite = Sprite.Create(texture, rect, pivot, MapTiles.PixelsPerUnit);
            sprite.name = "Mania Map Tile Sprite";
            return sprite;
        }

        /// <summary>
        /// Returns a new texture with the specified features.
        /// </summary>
        /// <param name="flags">The feature flags.</param>
        /// <param name="color">The tile background color.</param>
        private Texture2D CreateTexture(long flags, Color32 color)
        {
            var texture = new Texture2D(MapTiles.TileSize.x + 2, MapTiles.TileSize.y + 2);
            texture.name = "Mania Map Tile Texture";
            TextureUtility.Fill(texture, color);
            DrawMapTiles(texture, flags);
            TextureUtility.FillBorder(texture);
            texture.Apply();
            return texture;
        }

        /// <summary>
        /// Adds the map tiles for the specified tile flags to the texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="flags">The feature flags.</param>
        private void DrawMapTiles(Texture2D texture, long flags)
        {
            for (long i = flags; i != 0; i &= i - 1)
            {
                var flag = ~(i - 1) & i;
                var tile = MapTiles.GetTile(GetFeatureName(flag));
                TextureUtility.DrawImage(texture, tile, Vector2Int.one);
            }
        }
    }
}
