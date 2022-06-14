using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    public class LayoutTilemap : MonoBehaviour
    {
        [SerializeField]
        private Transform _container;
        public Transform Container { get => _container; set => _container = value; }
        
        [SerializeField]
        private MapTiles _mapTiles;
        /// <summary>
        /// The map tiles.
        /// </summary>
        public MapTiles MapTiles { get => _mapTiles; set => _mapTiles = value; }

        [SerializeField]
        private int _spritePadding = 4;
        public int SpritePadding { get => _spritePadding; set => _spritePadding = Mathf.Max(value, 0); }

        [SerializeField]
        private Vector2Int _textureSize = new Vector2Int(25, 25);
        public Vector2Int TextureSize { get => _textureSize; set => _textureSize = Vector2Int.Max(value, Vector2Int.one); }

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

        private bool TextureIsDirty { get; set; }

        private Texture2D Texture { get; set; }

        /// <summary>
        /// A dictionary of rendered managed textures.
        /// </summary>
        private Dictionary<MapTileHash, Sprite> Tiles { get; } = new Dictionary<MapTileHash, Sprite>();

        private void OnDestroy()
        {
            ReleaseSprites();
            Destroy(Texture);
        }

        private void OnValidate()
        {
            SpritePadding = SpritePadding;
            TextureSize = TextureSize;
        }

        public void ReleaseSprites()
        {
            foreach (var tile in Tiles.Values)
            {
                Destroy(tile);
            }

            Tiles.Clear();
        }

        private void CreateTexture()
        {
            var width = TextureSize.y * (MapTiles.TileSize.x + SpritePadding) + SpritePadding;
            var height = TextureSize.x * (MapTiles.TileSize.y + SpritePadding) + SpritePadding;

            if (Texture == null)
            {
                Texture = new Texture2D(width, height);
                ReleaseSprites();
            }
            else if (Texture.width != width || Texture.height != height)
            {
                Texture.Reinitialize(width, height);
                ReleaseSprites();
            } 
        }

        private Vector2Int NextSpritePosition()
        {
            var row = Tiles.Count / TextureSize.y;
            var column = Tiles.Count % TextureSize.y;
            var x = column * (MapTiles.TileSize.x + SpritePadding) + SpritePadding;
            var y = row * (MapTiles.TileSize.y + SpritePadding) + SpritePadding;
            return new Vector2Int(x, y);
        }

        private Sprite GetTile(MapTileTypes tileTypes, Color32 color)
        {
            var hash = new MapTileHash(tileTypes, color);

            if (!Tiles.TryGetValue(hash, out Sprite tile))
            {
                tile = CreateTile(tileTypes, color);
                Tiles.Add(hash, tile);
                TextureIsDirty = true;
            }

            return tile;
        }

        private Sprite CreateTile(MapTileTypes tileTypes, Color32 color)
        {
            var point = NextSpritePosition();
            var area = new RectInt(point, MapTiles.TileSize);
            TextureUtility.Fill(Texture, color, area);

            foreach (var flag in Bitwise.GetFlagEnumerable((int)tileTypes))
            {
                var type = (MapTileTypes)flag;
                var tile = MapTiles.GetTile(type);
                TextureUtility.DrawImage(Texture, tile, point);
            }

            var pivot = new Vector2(0.5f, 0.5f);
            var rect = new Rect(area.x, area.y, area.width, area.height);
            var pixelsPerUnit = Mathf.Min(MapTiles.TileSize.x, MapTiles.TileSize.y);
            return Sprite.Create(Texture, rect, pivot, pixelsPerUnit);
        }
    }
}