using MPewsey.Common.Mathematics;
using MPewsey.ManiaMap;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MPewsey.ManiaMapUnity.Drawing
{
    public class LayoutMapBase : MonoBehaviour
    {
        [SerializeField]
        private MapTileSet _mapTileSet;
        public MapTileSet MapTileSet { get => _mapTileSet; set => _mapTileSet = value; }

        [SerializeField]
        private DoorDrawMode _doorDrawMode = DoorDrawMode.AllDoors;
        public DoorDrawMode DoorDrawMode { get => _doorDrawMode; set => _doorDrawMode = value; }

        [SerializeField]
        private Color32 _roomColor = new Color32(75, 75, 75, 255);
        public Color32 RoomColor { get => _roomColor; set => _roomColor = value; }

        public Layout Layout { get; protected set; }
        public LayoutState LayoutState { get; protected set; }
        protected Dictionary<Uid, List<DoorPosition>> RoomDoors { get; set; } = new Dictionary<Uid, List<DoorPosition>>();
        protected Dictionary<int, List<Room>> RoomsByLayer { get; set; } = new Dictionary<int, List<Room>>();

        protected virtual void Initialize(Layout layout, LayoutState layoutState)
        {
            Layout = layout;
            LayoutState = layoutState;
            RoomDoors = layout.GetRoomDoors();
            RoomsByLayer = layout.GetRoomsByLayer();
        }

        protected bool DoorExists(Room room, Vector2DInt position, DoorDirection direction)
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

        protected long GetFeatureFlags(Cell cell)
        {
            long flags = 0;

            foreach (var tileName in cell.Features)
            {
                flags |= MapTileSet.GetFeatureFlag(tileName);
            }

            return flags;
        }

        protected Texture2D GetTile(Room room, Cell cell, Cell neighbor, Vector2DInt position, DoorDirection direction)
        {
            if (Door.ShowDoor(DoorDrawMode, direction) && cell.GetDoor(direction) != null && DoorExists(room, position, direction))
                return MapTileSet.GetTexture(MapTileType.GetDoorTileType(direction));

            if (neighbor == null)
                return MapTileSet.GetTexture(MapTileType.GetWallTileType(direction));

            return null;
        }

        protected long GetTileFlag(Room room, Cell cell, Cell neighbor, Vector2DInt position, DoorDirection direction)
        {
            if (Door.ShowDoor(DoorDrawMode, direction) && cell.GetDoor(direction) != null && DoorExists(room, position, direction))
                return MapTileSet.GetFeatureFlag(MapTileType.GetDoorTileType(direction));

            if (neighbor == null)
                return MapTileSet.GetFeatureFlag(MapTileType.GetWallTileType(direction));

            return 0;
        }

        protected void DrawFeatureTiles(Texture2D texture, Cell cell, Vector2Int point)
        {
            foreach (var tileName in cell.Features)
            {
                var tile = MapTileSet.GetTexture(tileName);
                TextureUtility.DrawImage(texture, tile, point);
            }
        }

        protected void DrawTiles(Texture2D texture, int z, RectangleInt bounds, Padding padding, Color32 backgroundColor)
        {
            TextureUtility.Fill(texture, backgroundColor);
            TextureUtility.TileImage(texture, MapTileSet.GetTexture(MapTileType.Grid));

            foreach (var room in RoomsByLayer[z])
            {
                var roomState = LayoutState?.RoomStates[room.Id];
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

        protected void SetTiles(Tilemap tilemap, int z)
        {
            tilemap.ClearAllTiles();

            foreach (var room in RoomsByLayer[z])
            {
                var roomState = LayoutState?.RoomStates[room.Id];
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