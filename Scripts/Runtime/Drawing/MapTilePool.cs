using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    public class MapTilePool : MonoBehaviour
    {
        private const int MaxFeatureCount = 64;

        [SerializeField]
        private MapTiles _mapTiles;
        public MapTiles MapTiles { get => _mapTiles; set => _mapTiles = value; }

        private Dictionary<string, long> FeatureFlags { get; } = new Dictionary<string, long>();
        private Dictionary<long, string> FeatureNames { get; } = new Dictionary<long, string>();
        private Dictionary<MapTileHash, Tile> Tiles { get; } = new Dictionary<MapTileHash, Tile>();

        private void Awake()
        {
            AddDefaultFeatures();
        }

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

        public void Clear()
        {
            Tiles.Clear();
            FeatureFlags.Clear();
            FeatureNames.Clear();
            AddDefaultFeatures();
        }

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

        public long GetFeatureFlag(string name)
        {
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
            var hash = new MapTileHash(flags, color);

            if (!Tiles.TryGetValue(hash, out Tile tile))
            {
                tile = CreateTile(flags, color);
                Tiles.Add(hash, tile);
            }

            return tile;
        }

        private Tile CreateTile(long flags, Color32 color)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.name = "Mania Map Tile";
            tile.sprite = CreateSprite(CreateTexture(flags, color));
            return tile;
        }

        private Sprite CreateSprite(Texture2D texture)
        {
            var pivot = new Vector2(0.5f, 0.5f);
            var rect = new Rect(1, 1, texture.width - 2, texture.height - 2);
            var sprite = Sprite.Create(texture, rect, pivot, MapTiles.PixelsPerUnit);
            sprite.name = "Mania Map Tile Sprite";
            return sprite;
        }

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
