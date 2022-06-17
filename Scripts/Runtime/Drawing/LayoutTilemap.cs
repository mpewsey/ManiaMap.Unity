using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    public class LayoutTilemap : MonoBehaviour
    {
        [SerializeField]
        private Grid _grid;
        public Grid Grid { get => _grid; set => _grid = value; }

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

        public void ClearTiles()
        {
            Tiles.Clear();
        }

        public void CreateGrid()
        {
            if (Grid == null)
            {
                var obj = new GameObject("Grid");
                Grid = obj.AddComponent<Grid>();
                obj.transform.SetParent(transform);
            }
        }

        private Tile GetTile(MapTileTypes tileTypes, Color32 color)
        {
            if (tileTypes == MapTileTypes.None)
                return null;

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
            DrawMapTiles(texture, tileTypes);
            texture.Apply();
            return texture;
        }

        private void DrawMapTiles(Texture2D texture, MapTileTypes tileTypes)
        {
            for (int i = (int)tileTypes; i != 0; i &= i - 1)
            {
                var flag = ~(i - 1) & i;
                var tile = MapTiles.GetTile((MapTileTypes)flag);
                TextureUtility.DrawImage(texture, tile, Vector2Int.zero);
            }
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

        public List<LayoutTilemapLayer> CreateLayers()
        {
            var manager = ManiaManager.Current;
            return CreateLayers(manager.Layout, manager.LayoutState);
        }

        public List<LayoutTilemapLayer> CreateLayers(Layout layout, LayoutState state = null)
        {
            Layout = layout;
            LayoutState = state;
            RoomDoors = layout.GetRoomDoors();
            return UpdateLayers();
        }

        private List<LayoutTilemapLayer> UpdateLayers()
        {
            var layers = CreateLayerComponents();

            foreach (var layer in layers)
            {
                DrawMap(layer.Tilemap, layer.Z);
            }

            return layers;
        }

        private List<LayoutTilemapLayer> CreateLayerComponents()
        {
            CreateGrid();
            var layers = Grid.GetComponentsInChildren<LayoutTilemapLayer>().ToList();
            var zs = new HashSet<int>(Layout.Rooms.Values.Select(x => x.Position.Z));

            // Destroy extra layers.
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                var layer = layers[i];

                if (!zs.Contains(layer.Z))
                {
                    Destroy(layer.gameObject);
                    layers.RemoveAt(i);
                }
            }

            // Create missing layers.
            foreach (var z in zs)
            {
                if (!layers.Any(x => x.Z == z))
                {
                    layers.Add(LayoutTilemapLayer.Create(z, Grid.transform));
                }
            }

            return layers;
        }

        private void DrawMap(Tilemap tilemap, int z)
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

                        // Accumulate map tile types
                        var tileTypes = MapTileTypes.None;
                        tileTypes |= GetTileType(room, cell, null, position, DoorDirection.Top);
                        tileTypes |= GetTileType(room, cell, null, position, DoorDirection.Bottom);
                        tileTypes |= GetTileType(room, cell, north, position, DoorDirection.North);
                        tileTypes |= GetTileType(room, cell, south, position, DoorDirection.South);
                        tileTypes |= GetTileType(room, cell, west, position, DoorDirection.West);
                        tileTypes |= GetTileType(room, cell, east, position, DoorDirection.East);

                        // Set the map tile.
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