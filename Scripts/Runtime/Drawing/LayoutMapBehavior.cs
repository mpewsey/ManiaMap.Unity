using MPewsey.Common.Mathematics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    /// <summary>
    /// A component for creating maps of Layout layers.
    /// </summary>
    public class LayoutMapBehavior : MonoBehaviour
    {
        [SerializeField]
        private Transform _layersContainer;
        /// <summary>
        /// The layers container.
        /// </summary>
        public Transform LayersContainer { get => _layersContainer; set => _layersContainer = value; }

        [SerializeField]
        private MapTiles _mapTiles;
        /// <summary>
        /// The map tiles.
        /// </summary>
        public MapTiles MapTiles { get => _mapTiles; set => _mapTiles = value; }

        [SerializeField]
        private FilterMode _filterMode = FilterMode.Point;
        /// <summary>
        /// The map texture filter mode.
        /// </summary>
        public FilterMode FilterMode { get => _filterMode; set => _filterMode = value; }

        [SerializeField]
        private bool _showDoors = true;
        /// <summary>
        /// If true, shows the doors on the map. Otherwise, only walls are shown.
        /// </summary>
        public bool ShowDoors { get => _showDoors; set => _showDoors = value; }

        [SerializeField]
        private Color32 _backgroundColor = Color.clear;
        /// <summary>
        /// The background color.
        /// </summary>
        public Color32 BackgroundColor { get => _backgroundColor; set => _backgroundColor = value; }

        [SerializeField]
        private Color32 _roomColor = new Color32(75, 75, 75, 255);
        /// <summary>
        /// The room color if visible.
        /// </summary>
        public Color32 RoomColor { get => _roomColor; set => _roomColor = value; }

        [SerializeField]
        private Padding _padding = new Padding(1);
        /// <summary>
        /// The tile padding to include around the plot.
        /// </summary>
        public Padding Padding { get => _padding; set => _padding = value; }

        /// <summary>
        /// A list of layout map layers.
        /// </summary>
        private List<LayoutMapLayer> Layers { get; set; } = new List<LayoutMapLayer>();

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
        /// The bounds of the layout.
        /// </summary>
        private RectangleInt LayoutBounds { get; set; }

        /// <summary>
        /// Returns a readonly list of layers.
        /// </summary>
        public IReadOnlyList<LayoutMapLayer> GetLayers()
        {
            return Layers;
        }

        /// <summary>
        /// Renders map images of all layout layers and saves them to the designated file path.
        /// The z (layer) values are added into the file paths before the file extension.
        /// </summary>
        /// <param name="path">The file path.</param>
        public void SaveLayerImages(string path)
        {
            CreateLayers();
            SaveCurrentLayerImages(path);
        }

        /// <summary>
        /// Renders map images of all layout layers and saves them to the designated file path.
        /// The z (layer) values are added into the file paths before the file extension.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="layout">The room layout.</param>
        /// <param name="state">The room layout state.</param>
        public void SaveLayerImages(string path, Layout layout, LayoutState state)
        {
            CreateLayers(layout, state);
            SaveCurrentLayerImages(path);
        }

        /// <summary>
        /// Saves the currently rendered layer images.
        /// </summary>
        /// <param name="path">The file path.</param>
        public void SaveCurrentLayerImages(string path)
        {
            var ext = Path.GetExtension(path);
            var name = Path.ChangeExtension(path, null);

            foreach (var layer in Layers)
            {
                var bytes = TextureUtility.EncodeToBytes(layer.Sprite.texture, ext);
                File.WriteAllBytes($"{name}_Z={layer.Z}{ext}", bytes);
            }
        }

        /// <summary>
        /// Instantiates the map's buffers.
        /// </summary>
        /// <param name="layout">The room layout.</param>
        /// <param name="state">The room layout state.</param>
        private void Initialize(Layout layout, LayoutState state)
        {
            Layout = layout;
            LayoutState = state;
            RoomDoors = layout.GetRoomDoors();
            LayoutBounds = layout.GetBounds();
        }

        /// <summary>
        /// Creates and draws the maps for the current layout.
        /// </summary>
        public void CreateLayers()
        {
            var manager = ManiaMapManager.Current;
            CreateLayers(manager.Layout, manager.LayoutState);
        }

        /// <summary>
        /// Creates and draws the maps for the layout.
        /// </summary>
        /// <param name="layout">The room layout.</param>
        /// <param name="state">The room layout state.</param>
        public void CreateLayers(Layout layout, LayoutState state)
        {
            Initialize(layout, state);
            var zs = Layout.Rooms.Values.Select(x => x.Position.Z).Distinct().ToList();
            zs.Sort();
            EnsureCapacity(zs.Count);

            // Draw the layers.
            var size = GetTextureSize();

            for (int i = 0; i < Layers.Count; i++)
            {
                var layer = Layers[i];
                layer.Initialize(size, zs[i]);
                DrawMap(layer.Sprite.texture, layer.Z);
            }
        }

        /// <summary>
        /// Destroys or creates layers until the specified size is met.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        private void EnsureCapacity(int capacity)
        {
            CreateLayersContainer();

            // Destroy extra layers.
            while (Layers.Count > capacity)
            {
                var index = Layers.Count - 1;
                Destroy(Layers[index].gameObject);
                Layers.RemoveAt(index);
            }

            // Create missing layers.
            while (Layers.Count < capacity)
            {
                Layers.Add(LayoutMapLayer.Create(this));
            }
        }

        /// <summary>
        /// If the layers container does not already exist, creates it and assigned it to the object.
        /// </summary>
        public void CreateLayersContainer()
        {
            if (LayersContainer == null)
            {
                var obj = new GameObject("Layers");
                obj.transform.SetParent(transform);
                LayersContainer = obj.transform;
            }
        }

        /// <summary>
        /// Returns the width and height of the texture in pixels.
        /// </summary>
        private Vector2Int GetTextureSize()
        {
            var width = MapTiles.TileSize.x * (Padding.Left + Padding.Right + LayoutBounds.Width);
            var height = MapTiles.TileSize.y * (Padding.Top + Padding.Bottom + LayoutBounds.Height);
            return new Vector2Int(width, height);
        }

        /// <summary>
        /// Draws the map onto the texture for the specified layer.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="z">The layer.</param>
        private void DrawMap(Texture2D texture, int z)
        {
            TextureUtility.Fill(texture, BackgroundColor);
            TextureUtility.TileImage(texture, MapTiles.GetTile(MapTileType.Grid));
            DrawMapTiles(texture, z);
            texture.Apply();
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
        /// Draws the map tiles onto the texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="z">The layer.</param>
        private void DrawMapTiles(Texture2D texture, int z)
        {
            foreach (var room in Layout.Rooms.Values)
            {
                // If room Z (layer) value is not equal, go to next room.
                if (room.Position.Z != z)
                    continue;

                var roomState = LayoutState?.RoomStates[room.Id];
                var cells = room.Template.Cells;
                var x0 = (room.Position.Y - LayoutBounds.X + Padding.Left) * MapTiles.TileSize.x;
                var y0 = (LayoutBounds.Height + Padding.Bottom - room.Position.X + LayoutBounds.Y - 1) * MapTiles.TileSize.y;

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
                        if (roomState != null && !roomState.IsVisible && !roomState.CellIsVisible(position))
                            continue;

                        // Calculate draw position
                        var x = x0 + MapTiles.TileSize.x * j;
                        var y = y0 - MapTiles.TileSize.y * i;
                        var point = new Vector2Int(x, y);

                        // Get adjacent cells
                        var north = cells.GetOrDefault(i - 1, j);
                        var south = cells.GetOrDefault(i + 1, j);
                        var west = cells.GetOrDefault(i, j - 1);
                        var east = cells.GetOrDefault(i, j + 1);

                        // Get the wall or door tiles
                        var topTile = GetTile(room, cell, null, position, DoorDirection.Top);
                        var bottomTile = GetTile(room, cell, null, position, DoorDirection.Bottom);
                        var northTile = GetTile(room, cell, north, position, DoorDirection.North);
                        var southTile = GetTile(room, cell, south, position, DoorDirection.South);
                        var westTile = GetTile(room, cell, west, position, DoorDirection.West);
                        var eastTile = GetTile(room, cell, east, position, DoorDirection.East);

                        // Add cell background fill
                        var rect = new RectInt(point, MapTiles.TileSize);
                        var color = roomState == null || roomState.CellIsVisible(position) ? ColorUtility.ConvertColor(room.Color) : RoomColor;
                        TextureUtility.CompositeFill(texture, color, rect);

                        // Superimpose applicable map tiles
                        TextureUtility.DrawImage(texture, northTile, point);
                        TextureUtility.DrawImage(texture, southTile, point);
                        TextureUtility.DrawImage(texture, westTile, point);
                        TextureUtility.DrawImage(texture, eastTile, point);
                        TextureUtility.DrawImage(texture, topTile, point);
                        TextureUtility.DrawImage(texture, bottomTile, point);

                        DrawFeatureTiles(texture, cell, point);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the feature tiles for the cell onto the texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="point">The tile position within the texture.</param>
        private void DrawFeatureTiles(Texture2D texture, ManiaMap.Cell cell, Vector2Int point)
        {
            foreach (var tileName in cell.Features)
            {
                var tile = MapTiles.GetTile(tileName);
                TextureUtility.DrawImage(texture, tile, point);
            }
        }

        /// <summary>
        /// Returns the map tile corresponding to the wall or door location.
        /// Returns null if the tile has neither a wall or door.
        /// </summary>
        /// <param name="room">The room.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="neighbor">The neighbor cell in the door direction. The neighbor can be null.</param>
        /// <param name="position">The local coordinate.</param>
        /// <param name="direction">The door direction.</param>
        private Texture2D GetTile(ManiaMap.Room room, ManiaMap.Cell cell, ManiaMap.Cell neighbor, Vector2DInt position, DoorDirection direction)
        {
            if (ShowDoors && cell.GetDoor(direction) != null && DoorExists(room, position, direction))
                return MapTiles.GetTile(MapTileType.GetDoorTileType(direction));

            if (neighbor == null)
                return MapTiles.GetTile(MapTileType.GetWallTileType(direction));

            return null;
        }
    }
}
