using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    /// <summary>
    /// References
    /// ----------
    /// * https://en.wikipedia.org/wiki/Alpha_compositing
    /// </summary>
    public class LayoutMap : MonoBehaviour
    {
        [SerializeField]
        private MapTiles _mapTiles;
        public MapTiles MapTiles { get => _mapTiles; set => _mapTiles = value; }

        [SerializeField]
        private Vector2Int _tileSize = new Vector2Int(16, 16);
        public Vector2Int TileSize { get => _tileSize; set => _tileSize = value; }

        [SerializeField]
        private Color32 _backgroundColor = Color.black;
        public Color32 BackgroundColor { get => _backgroundColor; set => _backgroundColor = value; }

        [SerializeField]
        private Padding _padding = new Padding(1);
        public Padding Padding { get => _padding; set => _padding = value; }

        private Layout Layout { get; set; }
        private LayoutState LayoutState { get; set; }
        private Dictionary<Uid, List<DoorPosition>> RoomDoors { get; set; }
        private System.Drawing.Rectangle LayoutBounds { get; set; }
        private Dictionary<int, Texture2D> MapLayers { get; } = new Dictionary<int, Texture2D>();

        private void OnDestroy()
        {
            ReleaseTextures();
        }

        public IEnumerable<KeyValuePair<int, Texture2D>> GetMapLayers()
        {
            return MapLayers;
        }

        public Texture2D GetMapLayer(int z)
        {
            return MapLayers[z];
        }

        private void ReleaseTextures()
        {
            foreach (var map in MapLayers.Values)
            {
                Destroy(map);
            }

            MapLayers.Clear();
        }

        public void Init(Layout layout, LayoutState state = null)
        {
            Layout = layout;
            LayoutState = state;
            LayoutBounds = layout.GetBounds();
            RoomDoors = layout.GetRoomDoors();
            ReleaseTextures();
        }

        /// <summary>
        /// Renders map images of all layout layers and saves them to the designated file path.
        /// The z (layer) values are added into the file paths before the file extension.
        /// </summary>
        /// <param name="path">The file path.</param>
        public void SaveImages(string path)
        {
            UpdateImages();
            var ext = Path.GetExtension(path);
            var name = Path.ChangeExtension(path, null);
            
            foreach (var pair in MapLayers)
            {
                var bytes = GetImageBytes(pair.Value, ext);
                File.WriteAllBytes($"{name}_Z={pair.Key}{ext}", bytes);
            }
        }

        private byte[] GetImageBytes(Texture2D texture, string ext)
        {
            switch (ext.ToLower())
            {
                case ".png":
                    return texture.EncodeToPNG();
                case ".jpg":
                    return texture.EncodeToJPG();
                default:
                    throw new ArgumentException($"Unhandled file extension: {ext}");
            }
        }

        public void UpdateImages()
        {
            var width = TileSize.x * (Padding.Left + Padding.Right + LayoutBounds.Width);
            var height = TileSize.y * (Padding.Top + Padding.Bottom + LayoutBounds.Height);
            var layers = new HashSet<int>(Layout.Rooms.Values.Select(x => x.Position.Z));

            foreach (var z in layers)
            {
                if (!MapLayers.TryGetValue(z, out Texture2D map))
                {
                    map = new Texture2D(width, height);
                    MapLayers.Add(z, map);
                }

                DrawMap(map, z);
            }
        }

        /// <summary>
        /// Calculates the composite of top color A onto bottom color B.
        /// </summary>
        /// <param name="colorA">The top color.</param>
        /// <param name="colorB">The bottom color.</param>
        private static Color CompositeColors(Color colorA, Color colorB)
        {
            var alpha1 = colorA.a;
            var alpha2 = colorB.a * (1 - colorA.a);
            var alpha = alpha1 + alpha2;
            alpha1 /= alpha;
            alpha2 /= alpha;
            var red = colorA.r * alpha1 + colorB.r * alpha2;
            var green = colorA.g * alpha1 + colorB.g * alpha2;
            var blue = colorA.b * alpha1 + colorB.b * alpha2;
            return new Color(red, green, blue, alpha);
        }

        private void DrawMap(Texture2D texture, int z)
        {
            FillBackground(texture);
            DrawGrid(texture);
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
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void FillBackground(Texture2D texture)
        {
            var pixels = texture.GetRawTextureData<Color32>();

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = BackgroundColor;
            }
        }

        private void DrawGrid(Texture2D texture)
        {
            var gridTile = MapTiles.GetTile(MapTileType.Grid);

            if (gridTile != null)
            {
                var grid = gridTile.GetRawTextureData<Color32>();
                var pixels = texture.GetRawTextureData<Color32>();

                for (int i = 0; i < texture.height; i++)
                {
                    for (int j = 0; j < texture.width; j++)
                    {
                        var index = i * texture.width + j;
                        var x = i % gridTile.height;
                        var y = j % gridTile.width;
                        var color = grid[x * gridTile.width + y];
                        pixels[index] = CompositeColors(color, pixels[index]);
                    }
                }
            }
        }

        private static Color32 ConvertColor(System.Drawing.Color color)
        {
            return new Color32(color.R, color.G, color.B, color.A);
        }

        private void DrawTileFill(Texture2D texture, Color32 color, Vector2Int point)
        {
            var pixels = texture.GetRawTextureData<Color32>();

            for (int i = 0; i < TileSize.y; i++)
            {
                for (int j = 0; j < TileSize.x; j++)
                {
                    var x = j + point.x;
                    var y = i + point.y;
                    var index = y * texture.width + x;
                    pixels[index] = CompositeColors(color, pixels[index]);
                }
            }
        }

        private void DrawMapTile(Texture2D texture, Texture2D tileTexture, Vector2Int point)
        {
            if (tileTexture == null)
                return;

            var pixels = texture.GetRawTextureData<Color32>();
            var tile = tileTexture.GetRawTextureData<Color32>();
            
            for (int i = 0; i < tileTexture.height; i++)
            {
                for (int j = 0; j < tileTexture.width; j++)
                {
                    var x = j + point.x;
                    var y = i + point.y;
                    var index = y * texture.width + x;
                    var color = tile[i * tileTexture.width + j];
                    pixels[index] = CompositeColors(color, pixels[index]);
                }
            }
        }

        private void DrawMapTiles(Texture2D texture, int z)
        {
            foreach (var room in Layout.Rooms.Values)
            {
                // If room Z (layer) value is not equal, go to next room.
                if (room.Position.Z != z)
                    continue;

                var roomState = LayoutState?.RoomStates[room.Id];
                var cells = room.Template.Cells;
                var x0 = (room.Position.Y - LayoutBounds.X + Padding.Left) * TileSize.x;
                var y0 = (room.Position.X - LayoutBounds.Y + Padding.Top) * TileSize.y;
                var color = ConvertColor(room.Color);

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
                        var x = TileSize.x * j + x0;
                        var y = TileSize.y * i + y0;
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
                        DrawTileFill(texture, color, point);

                        // Superimpose applicable map tiles
                        DrawMapTile(texture, northTile, point);
                        DrawMapTile(texture, southTile, point);
                        DrawMapTile(texture, westTile, point);
                        DrawMapTile(texture, eastTile, point);
                        DrawMapTile(texture, topTile, point);
                        DrawMapTile(texture, bottomTile, point);
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
        /// <returns></returns>
        private Texture2D GetTile(ManiaMap.Room room, ManiaMap.Cell cell, ManiaMap.Cell neighbor, Vector2DInt position, DoorDirection direction)
        {
            if (cell.GetDoor(direction) != null && DoorExists(room, position, direction))
                return MapTiles.GetTile(GetDoorTileType(direction));

            var wallType = GetWallTileType(direction);

            if (wallType != MapTileType.None && neighbor == null)
                return MapTiles.GetTile(wallType);

            return null;
        }

        /// <summary>
        /// Returns the door tile type corresponding to the direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <exception cref="ArgumentException">Raised if the direction is not handled.</exception>
        private static MapTileType GetDoorTileType(DoorDirection direction)
        {
            switch (direction)
            {
                case DoorDirection.North:
                    return MapTileType.NorthDoor;
                case DoorDirection.South:
                    return MapTileType.SouthDoor;
                case DoorDirection.East:
                    return MapTileType.EastDoor;
                case DoorDirection.West:
                    return MapTileType.WestDoor;
                case DoorDirection.Top:
                    return MapTileType.TopDoor;
                case DoorDirection.Bottom:
                    return MapTileType.BottomDoor;
                default:
                    throw new ArgumentException($"Unhandled direction: {direction}.");
            }
        }

        /// <summary>
        /// Returns the wall tile type corresponding to the direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <exception cref="ArgumentException">Raised if the direction is not handled.</exception>
        private static MapTileType GetWallTileType(DoorDirection direction)
        {
            switch (direction)
            {
                case DoorDirection.North:
                    return MapTileType.NorthWall;
                case DoorDirection.South:
                    return MapTileType.SouthWall;
                case DoorDirection.East:
                    return MapTileType.EastWall;
                case DoorDirection.West:
                    return MapTileType.WestWall;
                case DoorDirection.Top:
                case DoorDirection.Bottom:
                    return MapTileType.None;
                default:
                    throw new ArgumentException($"Unhandled direction: {direction}.");
            }
        }
    }
}
