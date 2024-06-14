using MPewsey.Common.Mathematics;
using MPewsey.ManiaMap;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MPewsey.ManiaMapUnity.Drawing
{
    /// <summary>
    /// The base class for layout maps.
    /// </summary>
    public abstract class LayoutMapBase : MonoBehaviour
    {
        [SerializeField]
        private MapTileSet _mapTileSet;
        /// <summary>
        /// The map tile set.
        /// </summary>
        public MapTileSet MapTileSet { get => _mapTileSet; set => _mapTileSet = value; }

        [SerializeField]
        private DoorDrawMode _doorDrawMode = DoorDrawMode.AllDoors;
        /// <summary>
        /// The option used for drawing doors for visible cells in the layout.
        /// </summary>
        public DoorDrawMode DoorDrawMode { get => _doorDrawMode; set => _doorDrawMode = value; }

        [SerializeField]
        private Color32 _roomColor = new Color32(75, 75, 75, 255);
        /// <summary>
        /// The room color used for a cell when a room is set as visible but the cell is not set as visible.
        /// This is useful for making certain rooms appear on the map before they have been visited.
        /// </summary>
        public Color32 RoomColor { get => _roomColor; set => _roomColor = value; }

        /// <summary>
        /// The layout pack used for the currently drawn map.
        /// </summary>
        public LayoutPack LayoutPack { get; protected set; }

        /// <summary>
        /// Returns a new layout state will all cells marked as visible.
        /// </summary>
        /// <param name="layout">The layout.</param>
        protected static LayoutState CreateFullyVisibleLayoutState(Layout layout)
        {
            var layoutState = new LayoutState(layout);

            foreach (var roomState in layoutState.RoomStates.Values)
            {
                System.Array.Fill(roomState.VisibleCells.Array, ~0);
            }

            return layoutState;
        }

        /// <summary>
        /// Returns a bit mask of all features assigned to the cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        protected long GetFeatureFlags(Cell cell)
        {
            long flags = 0;

            foreach (var tileName in cell.Features)
            {
                flags |= MapTileSet.GetFeatureFlag(tileName);
            }

            return flags;
        }

        /// <summary>
        /// Returns the tile texture for the door or wall at the specified location.
        /// Returns null if no door or wall exists.
        /// </summary>
        /// <param name="room">The room.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="neighbor">The cell neighbor in the specified direction.</param>
        /// <param name="position">The positional index of the cell within the room.</param>
        /// <param name="direction">The direction from the cell towards the neighbor.</param>
        protected Texture2D GetTile(Room room, Cell cell, Cell neighbor, Vector2DInt position, DoorDirection direction)
        {
            if (Door.ShowDoor(DoorDrawMode, direction) && cell.GetDoor(direction) != null && LayoutPack.DoorExists(room.Id, position, direction))
                return MapTileSet.GetTexture(MapTileType.GetDoorTileType(direction));

            if (neighbor == null)
                return MapTileSet.GetTexture(MapTileType.GetWallTileType(direction));

            return null;
        }

        /// <summary>
        /// Returns the door or wall bit mask for the specified location.
        /// Returns zero if no door or wall exists.
        /// </summary>
        /// <param name="room">The room.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="neighbor">The cell neighbor in the specified direction.</param>
        /// <param name="position">The positional index of the cell within the room.</param>
        /// <param name="direction">The direction from the cell towards the neighbor.</param>
        protected long GetTileFlag(Room room, Cell cell, Cell neighbor, Vector2DInt position, DoorDirection direction)
        {
            if (Door.ShowDoor(DoorDrawMode, direction) && cell.GetDoor(direction) != null && LayoutPack.DoorExists(room.Id, position, direction))
                return MapTileSet.GetFeatureFlag(MapTileType.GetDoorTileType(direction));

            if (neighbor == null)
                return MapTileSet.GetFeatureFlag(MapTileType.GetWallTileType(direction));

            return 0;
        }

        /// <summary>
        /// Draws the cell feature tiles onto the specified texture.
        /// </summary>
        /// <param name="texture">The canvas texture.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="point">The draw point from the bottom left of the texture.</param>
        protected void DrawFeatureTiles(Texture2D texture, Cell cell, Vector2Int point)
        {
            foreach (var tileName in cell.Features)
            {
                var tile = MapTileSet.GetTexture(tileName);
                TextureUtility.DrawImage(texture, tile, point);
            }
        }

        /// <summary>
        /// Draws the visible tiles for the specified layer (z) coordinate onto the texture.
        /// </summary>
        /// <param name="texture">The canvas texture.</param>
        /// <param name="z">The layer (z) coordinate.</param>
        /// <param name="padding">The canvas padding.</param>
        /// <param name="backgroundColor">The background color.</param>
        protected void DrawTiles(Texture2D texture, int z, Padding padding, Color32 backgroundColor)
        {
            var bounds = LayoutPack.LayoutBounds;
            TextureUtility.Fill(texture, backgroundColor);
            TextureUtility.TileImage(texture, MapTileSet.GetTexture(MapTileType.Grid));

            foreach (var room in LayoutPack.GetRoomsInLayer(z))
            {
                var roomState = LayoutPack.LayoutState?.RoomStates[room.Id];
                var cells = room.Template.Cells;
                var x0 = (room.Position.Y - bounds.X + padding.Left) * MapTileSet.TileSize.x;
                var y0 = (bounds.Height + padding.Bottom - room.Position.X + bounds.Y - 1) * MapTileSet.TileSize.y;

                for (int i = 0; i < cells.Rows; i++)
                {
                    for (int j = 0; j < cells.Columns; j++)
                    {
                        var cell = cells[i, j];

                        // If cell it empty, go to next cell.
                        if (cell == null)
                            continue;

                        var position = new Vector2DInt(i, j);
                        var isCompletelyVisible = roomState == null || roomState.CellIsVisible(position);

                        // If room state is defined and is not visible, go to next cell.
                        if (!isCompletelyVisible && !roomState.IsVisible)
                            continue;

                        // Calculate draw position
                        var x = x0 + MapTileSet.TileSize.x * j;
                        var y = y0 - MapTileSet.TileSize.y * i;
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
                        var rect = new RectInt(point, MapTileSet.TileSize);
                        var color = isCompletelyVisible ? ColorUtility.ConvertColor4ToColor32(room.Color) : RoomColor;
                        TextureUtility.CompositeFill(texture, color, rect);

                        // Superimpose applicable map tiles
                        TextureUtility.DrawImage(texture, northTile, point);
                        TextureUtility.DrawImage(texture, southTile, point);
                        TextureUtility.DrawImage(texture, westTile, point);
                        TextureUtility.DrawImage(texture, eastTile, point);
                        TextureUtility.DrawImage(texture, topTile, point);
                        TextureUtility.DrawImage(texture, bottomTile, point);

                        if (isCompletelyVisible)
                            DrawFeatureTiles(texture, cell, point);
                    }
                }
            }

            texture.Apply();
        }

        /// <summary>
        /// Sets the tiles for specified layer (z) coordinate onto the tilemap.
        /// </summary>
        /// <param name="tilemap">The tilemap.</param>
        /// <param name="z">The layer (z) coordinate.</param>
        protected void SetTiles(Tilemap tilemap, int z)
        {
            tilemap.ClearAllTiles();

            foreach (var room in LayoutPack.GetRoomsInLayer(z))
            {
                var roomState = LayoutPack.LayoutState?.RoomStates[room.Id];
                var cells = room.Template.Cells;

                for (int i = 0; i < cells.Rows; i++)
                {
                    for (int j = 0; j < cells.Columns; j++)
                    {
                        var cell = cells[i, j];

                        // If cell is empty, go to next cell.
                        if (cell == null)
                            continue;

                        var position = new Vector2DInt(i, j);
                        var isCompletelyVisible = roomState == null || roomState.CellIsVisible(position);

                        // If room state is defined and is not visible, go to next cell.
                        if (!isCompletelyVisible && !roomState.IsVisible)
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
                        var flags = GetTileFlag(room, cell, null, position, DoorDirection.Top);
                        flags |= GetTileFlag(room, cell, null, position, DoorDirection.Bottom);
                        flags |= GetTileFlag(room, cell, north, position, DoorDirection.North);
                        flags |= GetTileFlag(room, cell, south, position, DoorDirection.South);
                        flags |= GetTileFlag(room, cell, west, position, DoorDirection.West);
                        flags |= GetTileFlag(room, cell, east, position, DoorDirection.East);

                        if (isCompletelyVisible)
                            flags |= GetFeatureFlags(cell);

                        // Set the map tile.
                        var color = isCompletelyVisible ? ColorUtility.ConvertColor4ToColor32(room.Color) : RoomColor;
                        var tile = MapTileSet.GetTile(flags, color);
                        tilemap.SetTile(point, tile);
                    }
                }
            }
        }
    }
}