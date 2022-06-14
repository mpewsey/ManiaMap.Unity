using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    public class LayoutTilemap : MonoBehaviour
    {
        [SerializeField]
        private MapTiles _mapTiles;
        /// <summary>
        /// The map tiles.
        /// </summary>
        public MapTiles MapTiles { get => _mapTiles; set => _mapTiles = value; }

        /// <summary>
        /// The room layout.
        /// </summary>
        private Layout Layout { get; set; }

        /// <summary>
        /// A dictionary of room door positions by room ID.
        /// </summary>
        private LayoutState LayoutState { get; set; }

        /// <summary>
        /// A dictionary of 
        /// </summary>
        private Dictionary<Uid, List<DoorPosition>> RoomDoors { get; set; }

        /// <summary>
        /// A dictionary of rendered managed textures.
        /// </summary>
        private Dictionary<MapTileHash, Sprite> Tiles { get; } = new Dictionary<MapTileHash, Sprite>();

        private void OnDestroy()
        {
            ReleaseSprites();
        }

        public void ReleaseSprites()
        {
            foreach (var tile in Tiles.Values)
            {
                Destroy(tile.texture);
                Destroy(tile);
            }

            Tiles.Clear();
        }

        public Sprite GetTile(int tileType, Color32 color)
        {
            var hash = new MapTileHash(tileType, color);

            if (!Tiles.TryGetValue(hash, out Sprite tile))
            {
                tile = CreateTile(tileType, color);
                Tiles.Add(hash, tile);
            }

            return tile;
        }

        public Sprite CreateTile(int tileTypes, Color32 color)
        {
            var texture = new Texture2D(MapTiles.TileSize.x, MapTiles.TileSize.y);
            FillTile(texture, color);
            BlendTiles(texture, tileTypes);
            
            var size = new Vector2(texture.width, texture.height);
            var pivot = new Vector2(0.5f, 0.5f);
            var pixelsPerUnit = Mathf.Min(texture.width, texture.height);
            return Sprite.Create(texture, new Rect(Vector2.zero, size), pivot, pixelsPerUnit);
        }

        private void BlendTiles(Texture2D texture, int tileTypes)
        {
            for (int i = tileTypes; i != 0; i &= i - 1)
            {
                var type = ~(i - 1) & i;
                var tile = MapTiles.GetTile((MapTileType)type);
                BlendTiles(tile, texture);
            }
        }

        private static void FillTile(Texture2D texture, Color32 color)
        {
            var pixels = texture.GetRawTextureData<Color32>();

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
        }

        private static void BlendTiles(Texture2D topTile, Texture2D bottomTile)
        {
            var top = topTile.GetRawTextureData<Color32>();
            var bottom = bottomTile.GetRawTextureData<Color32>();

            for (int i = 0; i < top.Length; i++)
            {
                bottom[i] = ColorUtility.CompositeColors(top[i], bottom[i]);
            }
        }
    }
}