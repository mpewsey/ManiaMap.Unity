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
        private Dictionary<MapTileHash, Tile> Tiles { get; } = new Dictionary<MapTileHash, Tile>();

        private Dictionary<int, Tilemap> Tilemaps { get; } = new Dictionary<int, Tilemap>();

        public void ClearTiles()
        {
            Tiles.Clear();
        }

        private Tile GetTile(MapTileTypes tileTypes, Color32 color)
        {
            var hash = new MapTileHash(tileTypes, color);

            if (!Tiles.TryGetValue(hash, out Tile tile))
            {
                tile = CreateTile(tileTypes, color);
                Tiles.Add(hash, tile);
            }

            return tile;
        }

        private Texture2D CreateTexture(MapTileTypes tileTypes, Color32 color)
        {
            var texture = new Texture2D(MapTiles.TileSize.x, MapTiles.TileSize.y);
            TextureUtility.Fill(texture, color);

            foreach (var flag in FlagUtility.GetFlagEnumerable((int)tileTypes))
            {
                var type = (MapTileTypes)flag;
                var tile = MapTiles.GetTile(type);
                TextureUtility.DrawImage(texture, tile, Vector2Int.zero);
            }

            texture.Apply();
            return texture;
        }

        private Sprite CreateSprite(Texture2D texture)
        {
            var pivot = new Vector2(0.5f, 0.5f);
            var rect = new Rect(0, 0, texture.width, texture.height);
            return Sprite.Create(texture, rect, pivot, MapTiles.PixelsPerUnit);
        }

        private Tile CreateTile(MapTileTypes tileTypes, Color32 color)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = CreateSprite(CreateTexture(tileTypes, color));
            return tile;
        }

        /// <summary>
        /// Returns true if the door exists for the room.
        /// </summary>
        /// <param name="room">The room.</param>
        /// <param name="position">The local position of the door.</param>
        /// <param name="direction">The direction of the door.</param>
        private bool DoorExists(ManiaMap.Room room, Vector2DInt position, DoorDirection direction)
        {
            if (RoomDoors.TryGetValue(room.Id, out var doors))
            {
                foreach (var door in doors)
                {
                    if (door.Matches(position, direction))
                        return true;
                }
            }

            return false;
        }

        public Dictionary<int, Tilemap> CreateMaps(Layout layout, LayoutState state = null)
        {
            Layout = layout;
            LayoutState = state;
            RoomDoors = layout.GetRoomDoors();

            return new Dictionary<int, Tilemap>(Tilemaps);
        }

        private void DrawMapTiles(Tilemap tilemap, int z)
        {
            tilemap.ClearAllTiles();
            
            foreach (var room in Layout.Rooms.Values)
            {
                // If room Z (layer) value is not equal, go to next room.
                if (room.Position.Z != z)
                    continue;

                var roomState = LayoutState?.RoomStates[room.Id];
                var cells = room.Template.Cells;

                for (int i = 0; i < cells.Rows; i++)
                {
                    for (int j = 0; j < cells.Columns; j++)
                    {
                        var cell = cells[i, j];
                        var position = new Vector2DInt(i, j);

                        // If cell it empty, go to next cell.
                        if (cell == null)
                            continue;

                        // If room state is defined and is not visible, go to next cell.
                        if (roomState != null && !roomState.VisibleIndexes.Contains(position))
                            continue;

                        // Calculate draw position
                        var x = room.Position.Y + j;
                        var y = -room.Position.X - i;
                        var point = new Vector3Int(x, y, 0);

                        // Get adjacent cells
                        var north = cells.GetOrDefault(i - 1, j);
                        var south = cells.GetOrDefault(i + 1, j);
                        var west = cells.GetOrDefault(i, j - 1);
                        var east = cells.GetOrDefault(i, j + 1);

                        var tileTypes = MapTileTypes.None;
                        tileTypes |= GetTileType(room, cell, null, position, DoorDirection.Top);
                        tileTypes |= GetTileType(room, cell, null, position, DoorDirection.Bottom);
                        tileTypes |= GetTileType(room, cell, north, position, DoorDirection.North);
                        tileTypes |= GetTileType(room, cell, south, position, DoorDirection.South);
                        tileTypes |= GetTileType(room, cell, west, position, DoorDirection.West);
                        tileTypes |= GetTileType(room, cell, east, position, DoorDirection.East);

                        var tile = GetTile(tileTypes, ColorUtility.ConvertColor(room.Color));
                        tilemap.SetTile(point, tile);
                    }
                }
            }
        }

        private MapTileTypes GetTileType(ManiaMap.Room room, ManiaMap.Cell cell, ManiaMap.Cell neighbor, Vector2DInt position, DoorDirection direction)
        {
            if (cell.GetDoor(direction) != null && DoorExists(room, position, direction))
                return MapTiles.GetDoorTileType(direction);

            if (neighbor == null)
                return MapTiles.GetWallTileType(direction);

            return MapTileTypes.None;
        }
    }
}