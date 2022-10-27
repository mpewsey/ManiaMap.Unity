using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    /// <summary>
    /// A component for creating tilemaps of layout layers.
    /// </summary>
    public class LayoutTilemap : MonoBehaviour
    {
        [SerializeField]
        private Grid _grid;
        /// <summary>
        /// The grid container for the layers.
        /// </summary>
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
        /// A dictionary of map tiles by map tile type and color.
        /// </summary>
        public Dictionary<MapTileHash, Tile> Tiles { get; } = new Dictionary<MapTileHash, Tile>();

        /// <summary>
        /// Creates a new child grid component if it does not already exist.
        /// </summary>
        public void CreateGrid()
        {
            if (Grid == null)
            {
                var obj = new GameObject("Grid");
                Grid = obj.AddComponent<Grid>();
                obj.transform.SetParent(transform);
            }
        }

        /// <summary>
        /// Returns the tile corresponding to the tile types and color. If the tile
        /// does not already exist, creates and caches it.
        /// </summary>
        /// <param name="tileTypes">The tile types.</param>
        /// <param name="color">The tile color.</param>
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

        /// <summary>
        /// Creates a new texture for the specified tile types and color.
        /// </summary>
        /// <param name="tileTypes">The tile types.</param>
        /// <param name="color">The tile color.</param>
        private Texture2D CreateTexture(MapTileTypes tileTypes, Color32 color)
        {
            var texture = new Texture2D(MapTiles.TileSize.x + 2, MapTiles.TileSize.y + 2);
            texture.name = "Mania Map Tile Texture";
            TextureUtility.Fill(texture, color);
            DrawMapTiles(texture, tileTypes);
            texture.Apply();
            return texture;
        }

        /// <summary>
        /// Draws the map tiles for the specified tile types onto the texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="tileTypes">The tile types to draw.</param>
        private void DrawMapTiles(Texture2D texture, MapTileTypes tileTypes)
        {
            for (int i = (int)tileTypes; i != 0; i &= i - 1)
            {
                var flag = ~(i - 1) & i;
                var tile = MapTiles.GetTile((MapTileTypes)flag);
                TextureUtility.DrawImage(texture, tile, Vector2Int.one);
                TextureUtility.FillBorder(texture);
            }
        }

        /// <summary>
        /// Creates a new sprite from the specified tile texture.
        /// </summary>
        /// <param name="texture">The tile texture.</param>
        private Sprite CreateSprite(Texture2D texture)
        {
            var pivot = new Vector2(0.5f, 0.5f);
            var rect = new Rect(1, 1, texture.width - 2, texture.height - 2);
            var sprite = Sprite.Create(texture, rect, pivot, MapTiles.PixelsPerUnit);
            sprite.name = "Mania Map Tile Sprite";
            return sprite;
        }

        /// <summary>
        /// Creates a new tile for the specified tile types and color.
        /// </summary>
        /// <param name="tileTypes">The tile types.</param>
        /// <param name="color">The tile color.</param>
        private Tile CreateTile(MapTileTypes tileTypes, Color32 color)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.name = "Mania Map Tile";
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

        /// <summary>
        /// Initializes the tilemap buffers.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="state">The layout state.</param>
        private void Initialize(Layout layout, LayoutState state)
        {
            Layout = layout;
            LayoutState = state;
            RoomDoors = layout.GetRoomDoors();
        }

        /// <summary>
        /// Creates layer tilemaps for the current layout and returns a list of layers.
        /// </summary>
        public List<LayoutTilemapLayer> CreateLayers()
        {
            var manager = ManiaMapManager.Current;
            return CreateLayers(manager.Layout, manager.LayoutState);
        }

        /// <summary>
        /// Creates layer tilemaps for a layout and returns a list of layers.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="state">The layout state.</param>
        public List<LayoutTilemapLayer> CreateLayers(Layout layout, LayoutState state = null)
        {
            Initialize(layout, state);
            var layers = CreateLayerComponents();

            foreach (var layer in layers)
            {
                DrawMap(layer.Tilemap, layer.Z);
            }

            return layers;
        }

        /// <summary>
        /// Creates the layers required for the layout. Extra layers are destroyed.
        /// </summary>
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
                    layers.Add(LayoutTilemapLayer.Create(this, z));
                }
            }

            return layers;
        }

        /// <summary>
        /// Adds the map tiles for the layer to the tilemap.
        /// </summary>
        /// <param name="tilemap">The tilemap.</param>
        /// <param name="z">The layer value.</param>
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
                        if (roomState != null && !roomState.CellIsVisible(position))
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

        /// <summary>
        /// Returns the tile type for the cell connection.
        /// </summary>
        /// <param name="room">The room.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="neighbor">The neighboring cell.</param>
        /// <param name="position">The cell position.</param>
        /// <param name="direction">The door direction.</param>
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