using MPewsey.Common.Mathematics;
using MPewsey.ManiaMap.Unity.Exceptions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    /// <summary>
    /// A component for creating tilemaps of layout layers.
    /// </summary>
    [RequireComponent(typeof(MapTilePool))]
    public class LayoutTilemapBehavior : MonoBehaviour, IOnionMapTarget
    {
        [SerializeField]
        private Grid _grid;
        /// <summary>
        /// The grid container for the layers.
        /// </summary>
        public Grid Grid { get => _grid; set => _grid = value; }

        [SerializeField]
        private DoorDrawMode _doorDrawMode = DoorDrawMode.AllDoors;
        /// <summary>
        /// An option controlling which doors will be drawn.
        /// </summary>
        public DoorDrawMode DoorDrawMode { get => _doorDrawMode; set => _doorDrawMode = value; }

        [SerializeField]
        private Color32 _roomColor = new Color32(75, 75, 75, 255);
        /// <summary>
        /// The room color if visible.
        /// </summary>
        public Color32 RoomColor { get => _roomColor; set => _roomColor = value; }

        /// <summary>
        /// A list of tilemap layers.
        /// </summary>
        private List<LayoutTilemapLayer> Layers { get; set; } = new List<LayoutTilemapLayer>();

        private Layout _layout;
        /// <summary>
        /// The room layout.
        /// </summary>
        private Layout Layout
        {
            get
            {
                AssertIsInitialized();
                return _layout;
            }
            set => _layout = value;
        }

        private LayoutState _layoutState;
        /// <summary>
        /// A dictionary of room door positions by room ID.
        /// </summary>
        private LayoutState LayoutState
        {
            get
            {
                AssertIsInitialized();
                return _layoutState;
            }
            set => _layoutState = value;
        }

        private Dictionary<Uid, List<DoorPosition>> _roomDoors;
        /// <summary>
        /// A dictionary of door positions by their containing room.
        /// </summary>
        private Dictionary<Uid, List<DoorPosition>> RoomDoors
        {
            get
            {
                AssertIsInitialized();
                return _roomDoors;
            }
            set => _roomDoors = value;
        }

        private int[] _layerPositions;
        /// <summary>
        /// An array of sorted layer positions.
        /// </summary>
        private int[] LayerPositions
        {
            get
            {
                AssertIsInitialized();
                return _layerPositions;
            }
            set => _layerPositions = value;
        }

        /// <summary>
        /// The attached map tile pool component.
        /// </summary>
        public MapTilePool MapTilePool { get; private set; }

        /// <summary>
        /// True if the map is initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        private void Awake()
        {
            MapTilePool = GetComponent<MapTilePool>();
        }

        /// <summary>
        /// Returns a readonly list of layers.
        /// </summary>
        public IReadOnlyList<LayoutTilemapLayer> GetLayers() => Layers;

        /// <inheritdoc/>
        IEnumerable<IOnionMapLayer> IOnionMapTarget.Layers() => Layers;

        /// <inheritdoc/>
        public Vector2 LayerRange()
        {
            var positions = LayerPositions;

            if (positions.Length == 0)
                return Vector2.zero;

            return new Vector2(positions[0], positions[positions.Length - 1]);
        }

        /// <summary>
        /// Initializes the map.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="state">The layout state.</param>
        public void Initialize(Layout layout, LayoutState state = null)
        {
            Layout = layout;
            LayoutState = state;
            RoomDoors = layout.GetRoomDoors();
            LayerPositions = DistinctLayerPositions(layout);
            IsInitialized = true;
        }

        /// <summary>
        /// Initializes the map from the current ManiaMapManager.
        /// </summary>
        public void InitializeFromManager()
        {
            var manager = ManiaMapManager.Current;
            Initialize(manager.Layout, manager.LayoutState);
        }

        /// <summary>
        /// Returns an array of sorted distinct layer positions for the layout.
        /// </summary>
        /// <param name="layout">The layout.</param>
        private static int[] DistinctLayerPositions(Layout layout)
        {
            var result = layout.Rooms.Values.Select(x => x.Position.Z).Distinct().ToArray();
            System.Array.Sort(result);
            return result;
        }

        /// <summary>
        /// Checks that the map is initialized and raises an exception if it isn't.
        /// </summary>
        /// <exception cref="LayoutMapNotInitializedException">Raised if the map is not initialized.</exception>
        private void AssertIsInitialized()
        {
            if (!IsInitialized)
                throw new LayoutMapNotInitializedException($"Attempting to access initialized method when not initialized: {this}.");
        }

        /// <summary>
        /// Destroys all layers and clears the layers list.
        /// </summary>
        public void Clear()
        {
            foreach (var layer in Layers)
            {
                Destroy(layer.gameObject);
            }

            Layers.Clear();
        }

        /// <summary>
        /// Creates a new child grid component if it does not already exist.
        /// </summary>
        public void CreateGrid()
        {
            if (Grid == null)
            {
                var obj = new GameObject("Grid");
                obj.transform.SetParent(transform);
                Grid = obj.AddComponent<Grid>();
            }
        }

        /// <summary>
        /// Returns true if the door exists for the room.
        /// </summary>
        /// <param name="room">The room.</param>
        /// <param name="position">The local position of the door.</param>
        /// <param name="direction">The direction of the door.</param>
        private bool DoorExists(Room room, Vector2DInt position, DoorDirection direction)
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
        /// Creates layer tilemaps for a layout.
        /// </summary>
        public void Draw()
        {
            CreateLayers();

            for (int i = 0; i < Layers.Count; i++)
            {
                var layer = Layers[i];
                layer.Initialize(LayerPositions[i]);
                DrawMap(layer.Tilemap, layer.Z);
            }
        }

        /// <summary>
        /// Destroys or creates layers until the specified size is met.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        private void CreateLayers()
        {
            CreateGrid();
            var capacity = LayerPositions.Length;

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
                Layers.Add(LayoutTilemapLayer.Create(this));
            }
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

                        // If cell it empty, go to next cell.
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
                        var color = isCompletelyVisible ? ColorUtility.ConvertColor(room.Color) : RoomColor;
                        var tile = MapTilePool.GetTile(flags, color);
                        tilemap.SetTile(point, tile);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the feature flags for the cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        private long GetFeatureFlags(Cell cell)
        {
            long flags = 0;

            foreach (var tileName in cell.Features)
            {
                flags |= MapTilePool.GetFeatureFlag(tileName);
            }

            return flags;
        }

        /// <summary>
        /// Returns the tile flag for the cell connection.
        /// </summary>
        /// <param name="room">The room.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="neighbor">The neighboring cell.</param>
        /// <param name="position">The cell position.</param>
        /// <param name="direction">The door direction.</param>
        private long GetTileFlag(Room room, Cell cell, Cell neighbor, Vector2DInt position, DoorDirection direction)
        {
            if (Door.ShowDoor(DoorDrawMode, direction) && cell.GetDoor(direction) != null && DoorExists(room, position, direction))
                return MapTilePool.GetFeatureFlag(MapTileType.GetDoorTileType(direction));

            if (neighbor == null)
                return MapTilePool.GetFeatureFlag(MapTileType.GetWallTileType(direction));

            return 0;
        }
    }
}