using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    /// <summary>
    /// A component for creating maps of Layout layers.
    /// </summary>
    public class LayoutMap : MonoBehaviour
    {
        [SerializeField]
        private Transform _layerContainer;
        public Transform LayerContainer { get => _layerContainer; set => _layerContainer = value; }
        
        [SerializeField]
        private MapTiles _mapTiles;
        /// <summary>
        /// The map tiles.
        /// </summary>
        public MapTiles MapTiles { get => _mapTiles; set => _mapTiles = value; }

        [SerializeField]
        private Color32 _backgroundColor = Color.clear;
        /// <summary>
        /// The background color.
        /// </summary>
        public Color32 BackgroundColor { get => _backgroundColor; set => _backgroundColor = value; }

        [SerializeField]
        private Padding _padding = new Padding(1);
        /// <summary>
        /// The tile padding to include around the plot.
        /// </summary>
        public Padding Padding { get => _padding; set => _padding = value; }

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
        private System.Drawing.Rectangle LayoutBounds { get; set; }

        /// <summary>
        /// A dictionary of rendered layer textures.
        /// </summary>
        private Dictionary<int, Texture2D> Textures { get; } = new Dictionary<int, Texture2D>();

        public void ClearTextures()
        {
            Textures.Clear();
        }

        public void CreateContainer()
        {
            if (LayerContainer == null)
            {
                var obj = new GameObject("Layers");
                obj.transform.SetParent(transform);
                LayerContainer = obj.transform;
            }
        }

        /// <summary>
        /// Renders map images of all layout layers and saves them to the designated file path.
        /// The z (layer) values are added into the file paths before the file extension.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="layout">The room layout.</param>
        /// <param name="state">The room layout state.</param>
        public void SaveImages(string path, Layout layout, LayoutState state = null)
        {
            var ext = Path.GetExtension(path);
            var name = Path.ChangeExtension(path, null);
            var layers = CreateImages(layout, state);

            foreach (var pair in layers)
            {
                var bytes = TextureUtility.EncodeToBytes(pair.Value, ext);
                File.WriteAllBytes($"{name}_Z={pair.Key}{ext}", bytes);
            }
        }

        /// <summary>
        /// Populates the map layers dictionary with textures. Textures for layers
        /// not present in the layout are destroyed, while missing layer textures
        /// are added.
        /// </summary>
        private void CreateTextures()
        {
            var size = GetTextureSize();
            var layers = new HashSet<int>(Layout.Rooms.Values.Select(x => x.Position.Z));
            var removeLayers = Textures.Keys.Where(x => !layers.Contains(x)).ToList();

            foreach (var z in removeLayers)
            {
                Textures.Remove(z);
            }

            foreach (var z in layers)
            {
                if (!Textures.TryGetValue(z, out Texture2D map))
                    Textures.Add(z, new Texture2D(size.x, size.y));
                else if (map.width != size.x || map.height != size.y)
                    map.Reinitialize(size.x, size.y);
            }
        }

        /// <summary>
        /// Returns a dictionary of layer map textures for the layout.
        /// </summary>
        /// <param name="layout">The room layout.</param>
        /// <param name="state">The room layout state.</param>
        public Dictionary<int, Texture2D> CreateImages(Layout layout, LayoutState state = null)
        {
            Layout = layout;
            LayoutState = state;
            RoomDoors = layout.GetRoomDoors();
            LayoutBounds = layout.GetBounds();
            CreateTextures();

            foreach (var pair in Textures)
            {
                DrawMap(pair.Value, pair.Key);
            }

            return new Dictionary<int, Texture2D>(Textures);
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
            TextureUtility.TileImage(texture, MapTiles.GetTile(MapTileTypes.Grid));
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
                        if (roomState != null && !roomState.VisibleIndexes.Contains(position))
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
                        TextureUtility.CompositeFill(texture, ColorUtility.ConvertColor(room.Color), rect);

                        // Superimpose applicable map tiles
                        TextureUtility.DrawImage(texture, northTile, point);
                        TextureUtility.DrawImage(texture, southTile, point);
                        TextureUtility.DrawImage(texture, westTile, point);
                        TextureUtility.DrawImage(texture, eastTile, point);
                        TextureUtility.DrawImage(texture, topTile, point);
                        TextureUtility.DrawImage(texture, bottomTile, point);
                    }
                }
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
            if (cell.GetDoor(direction) != null && DoorExists(room, position, direction))
                return MapTiles.GetDoorTile(direction);

            if (neighbor == null)
                return MapTiles.GetWallTile(direction);

            return null;
        }
    }
}
