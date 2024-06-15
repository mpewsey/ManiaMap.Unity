using MPewsey.Common.Collections;
using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Exceptions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// A component for creating a room.
    /// 
    /// The room contains methods for various coordinate systems:
    /// 
    /// * Grid Coordinates - The origin is in the top left of the room. Positive X is right in the direction of increasing columns. Positive Y is down in the direction of increasing rows. Looking at the XY plane, positive Z points out towards the viewer.
    /// * Local Coordinates - The coordinates assuming the room's transform position is the origin. Axes are in the same direction as Unity's. Coordinates are assumed to be axis aligned (not rotated).
    /// * Global Coordinates - The coordinates in Unity's global coordinates. Coordinates are assumed to be axis aligned (not rotated).
    /// 
    /// Cell related calculations are generally performed within Grid Coordinate space since that is consistent between room types.
    /// </summary>
    public class RoomComponent : MonoBehaviour
    {
        [SerializeField]
        private RoomTemplateResource _roomTemplate;
        /// <summary>
        /// The room template.
        /// </summary>
        public RoomTemplateResource RoomTemplate { get => _roomTemplate; set => _roomTemplate = value; }

        [SerializeField]
        private string _name = "<None>";
        /// <summary>
        /// The room name.
        /// </summary>
        public string Name { get => _name; set => _name = value; }

        [SerializeField]
        private RoomType _roomType;
        /// <summary>
        /// The room type.
        /// </summary>
        public RoomType RoomType { get => _roomType; set => _roomType = value; }

        [SerializeField]
        private Vector2Int _size = Vector2Int.one;
        /// <summary>
        /// The size of the room grid in rows (x) and columns (y).
        /// </summary>
        public Vector2Int Size { get => _size; set => SetSizeField(ref _size, value); }

        [SerializeField]
        private Vector3 _cellSize = new Vector3(1, 1, 0.001f);
        /// <summary>
        /// The size of each cell within the grid coordinate system.
        /// </summary>
        public Vector3 CellSize { get => _cellSize; set => _cellSize = Vector3.Max(value, 0.001f * Vector3.one); }

        [HideInInspector]
        [SerializeField]
        private List<ActiveCellsRow> _activeCells = new List<ActiveCellsRow>();
        /// <summary>
        /// A list of active cell rows.
        /// </summary>
        public List<ActiveCellsRow> ActiveCells { get => _activeCells; set => _activeCells = value; }

        [SerializeField]
        private UnityEvent _onInitialize = new UnityEvent();
        /// <summary>
        /// An event invoked when the room is initialized.
        /// </summary>
        public UnityEvent OnInitialize { get => _onInitialize; set => _onInitialize = value; }

        [SerializeField]
        private UnityEvent _onInitialized = new UnityEvent();
        /// <summary>
        /// The event invoked after the room has completed initialization.
        /// </summary>
        public UnityEvent OnInitialized { get => _onInitialized; set => _onInitialized = value; }

        [SerializeField]
        private CellAreaTriggerEvent _onCellAreaEntered = new CellAreaTriggerEvent();
        /// <summary>
        /// The event invoked when a room cell has been entered. Message includes the enterered cell and the colliding GameObject.
        /// </summary>
        public CellAreaTriggerEvent OnCellAreaEntered { get => _onCellAreaEntered; set => _onCellAreaEntered = value; }

        [SerializeField]
        private CellAreaTriggerEvent _onCellAreaExited = new CellAreaTriggerEvent();
        /// <summary>
        /// The event invoked when a room cell has been exited. Message includes the exited cell and the colliding GameObject.
        /// </summary>
        public CellAreaTriggerEvent OnCellAreaExited { get => _onCellAreaExited; set => _onCellAreaExited = value; }

        /// <summary>
        /// True if the room has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// The layout pack.
        /// </summary>
        public LayoutPack LayoutPack { get; private set; }

        /// <summary>
        /// The room data.
        /// </summary>
        public Room RoomLayout { get; private set; }

        /// <summary>
        /// The room state.
        /// </summary>
        public RoomState RoomState { get; private set; }

        /// <summary>
        /// The row index.
        /// </summary>
        public int Rows { get => Size.x; set => Size = new Vector2Int(value, Size.y); }

        /// <summary>
        /// The column index.
        /// </summary>
        public int Columns { get => Size.y; set => Size = new Vector2Int(Size.x, value); }

        private void SetSizeField(ref Vector2Int field, Vector2Int value)
        {
            field = Vector2Int.Max(value, Vector2Int.one);
            SizeActiveCells();
        }

        private void OnValidate()
        {
            Size = Size;
            CellSize = CellSize;
        }

        private void OnDrawGizmos()
        {
            var activeFillColor = new Color(0, 0, 1, 0.2f);
            var inactiveFillColor = new Color(1, 0, 0, 0.2f);
            var lineColor = new Color(0, 0, 0);
            DrawCells(activeFillColor, lineColor, true);
            DrawCells(inactiveFillColor, lineColor, false);
        }

        /// <summary>
        /// Draws the cell cube gizmos for the specified cell activity.
        /// </summary>
        /// <param name="fillColor">The gizmo fill color.</param>
        /// <param name="lineColor">The gizmo line color.</param>
        /// <param name="activity">The activity of the cells to draw.</param>
        private void DrawCells(Color fillColor, Color lineColor, bool activity)
        {
            var cellSize = GridToLocalPosition(CellSize);

            for (int i = 0; i < ActiveCells.Count; i++)
            {
                var row = ActiveCells[i].Values;

                for (int j = 0; j < row.Count; j++)
                {
                    if (row[j] == activity)
                    {
                        var center = CellCenterGlobalPosition(i, j);
                        Gizmos.color = fillColor;
                        Gizmos.DrawCube(center, cellSize);
                        Gizmos.color = lineColor;
                        Gizmos.DrawWireCube(center, cellSize);
                    }
                }
            }
        }

        /// <summary>
        /// Instantiates and initializes a room asynchronously. Returns the operation handle.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="layoutPack">The layout pack.</param>
        /// <param name="prefab">The room prefab asset reference.</param>
        /// <param name="parent">The parent transform.</param>
        /// <param name="assignLayoutPosition">If true, the room will have its local position moved to its location within the layout.</param>
        public static AsyncOperationHandle<GameObject> InstantiateRoomAsync(Uid id, LayoutPack layoutPack,
            AssetReferenceGameObject prefab, Transform parent = null,
            bool assignLayoutPosition = false)
        {
            var roomLayout = layoutPack.Layout.Rooms[id];
            var roomState = layoutPack.LayoutState.RoomStates[id];
            var cellLayer = layoutPack.Settings.CellLayer;
            var triggeringLayers = layoutPack.Settings.TriggeringLayers;
            return InstantiateRoomAsync(prefab, parent, layoutPack, roomLayout, roomState, cellLayer, triggeringLayers, assignLayoutPosition);
        }

        /// <summary>
        /// Instantiates and initializes a room asynchronously. Returns the operation handle.
        /// </summary>
        /// <param name="prefab">The room prefab asset reference.</param>
        /// <param name="parent">The parent transform.</param>
        /// <param name="layoutPack">The layout pack.</param>
        /// <param name="roomLayout">The room's layout.</param>
        /// <param name="roomState">The room's layout state.</param>
        /// <param name="cellLayer">The physics layer assigned to cell triggers.</param>
        /// <param name="triggeringLayers">The physics layers that trigger the room cell triggers.</param>
        /// <param name="assignLayoutPosition">If true, the room will have its local position moved to its location within the layout.</param>
        public static AsyncOperationHandle<GameObject> InstantiateRoomAsync(AssetReferenceGameObject prefab, Transform parent,
            LayoutPack layoutPack, Room roomLayout, RoomState roomState,
            int cellLayer, LayerMask triggeringLayers, bool assignLayoutPosition)
        {
            var handle = prefab.InstantiateAsync(parent);

            handle.Completed += handle => handle.Result.GetComponent<RoomComponent>()
                .Initialize(layoutPack, roomLayout, roomState, cellLayer, triggeringLayers, assignLayoutPosition);

            return handle;
        }

        /// <summary>
        /// Instantiates and initializes a room and returns it.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="layoutPack">The layout pack.</param>
        /// <param name="prefab">The room prefab.</param>
        /// <param name="parent">The parent transform.</param>
        /// <param name="assignLayoutPosition">If true, the room will have its local position moved to its location within the layout.</param>
        public static RoomComponent InstantiateRoom(Uid id, LayoutPack layoutPack, GameObject prefab, Transform parent = null,
            bool assignLayoutPosition = false)
        {
            var roomLayout = layoutPack.Layout.Rooms[id];
            var roomState = layoutPack.LayoutState.RoomStates[id];
            var cellLayer = layoutPack.Settings.CellLayer;
            var triggeringLayers = layoutPack.Settings.TriggeringLayers;
            return InstantiateRoom(prefab, parent, layoutPack, roomLayout, roomState, cellLayer, triggeringLayers, assignLayoutPosition);
        }

        /// <summary>
        /// Instantiates and initializes a room and returns it.
        /// </summary>
        /// <param name="prefab">The room prefab.</param>
        /// <param name="parent">The parent transform.</param>
        /// <param name="layoutPack">The layout pack.</param>
        /// <param name="roomLayout">The room's layout.</param>
        /// <param name="roomState">The room's layout state.</param>
        /// <param name="cellLayer">The physics layer assigned to cell triggers.</param>
        /// <param name="triggeringLayers">The physics layers that trigger the room cell triggers.</param>
        /// <param name="assignLayoutPosition">If true, the room will have its local position moved to its location within the layout.</param>
        public static RoomComponent InstantiateRoom(GameObject prefab, Transform parent,
            LayoutPack layoutPack, Room roomLayout, RoomState roomState,
            int cellLayer, LayerMask triggeringLayers, bool assignLayoutPosition)
        {
            var room = Instantiate(prefab, parent).GetComponent<RoomComponent>();
            room.Initialize(layoutPack, roomLayout, roomState, cellLayer, triggeringLayers, assignLayoutPosition);
            return room;
        }

        /// <summary>
        /// Initializes the room. Returns false if the room has already been initialized.
        /// </summary>
        /// <param name="layoutPack">The layout pack.</param>
        /// <param name="roomLayout">The room's layout.</param>
        /// <param name="roomState">The room's layout state.</param>
        /// <param name="cellLayer">The physics layer assigned to cell triggers.</param>
        /// <param name="triggeringLayers">The physics layers that trigger the room cell triggers.</param>
        /// <param name="assignLayoutPosition">If true, the room will have its local position moved to its location within the layout.</param>
        public bool Initialize(LayoutPack layoutPack, Room roomLayout, RoomState roomState,
            int cellLayer, LayerMask triggeringLayers, bool assignLayoutPosition)
        {
            if (IsInitialized)
                return false;

            LayoutPack = layoutPack;
            RoomLayout = roomLayout;
            RoomState = roomState;

            if (assignLayoutPosition)
                MoveToLayoutPosition();

            CreateCellAreas(cellLayer, triggeringLayers);
            IsInitialized = true;
            OnInitialize.Invoke();
            OnInitialized.Invoke();
            return true;
        }

        /// <summary>
        /// Sets the room's local position to its position in the layout.
        /// </summary>
        private void MoveToLayoutPosition()
        {
            var x = CellSize.x * RoomLayout.Position.Y;
            var y = CellSize.y * RoomLayout.Position.X;
            var z = CellSize.z * RoomLayout.Position.Z;
            transform.localPosition = GridToLocalPosition(new Vector3(x, y, z));
        }

        /// <summary>
        /// Creates the cell area triggers as children of the room.
        /// </summary>
        /// <param name="cellLayer">The physics layer assigned to cell triggers.</param>
        /// <param name="triggeringLayers">The physics layers that trigger the room cell triggers.</param>
        private void CreateCellAreas(int cellLayer, LayerMask triggeringLayers)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (GetCellActivity(i, j))
                        CellArea.InstantiateCellArea(i, j, this, cellLayer, triggeringLayers);
                }
            }
        }

        /// <summary>
        /// Sizes the active cells list to match the room size.
        /// </summary>
        public void SizeActiveCells()
        {
            while (ActiveCells.Count > Rows)
            {
                ActiveCells.RemoveAt(ActiveCells.Count - 1);
            }

            foreach (var row in ActiveCells)
            {
                while (row.Values.Count > Columns)
                {
                    row.Values.RemoveAt(row.Values.Count - 1);
                }

                while (row.Values.Count < Columns)
                {
                    row.Values.Add(true);
                }
            }

            while (ActiveCells.Count < Rows)
            {
                ActiveCells.Add(new ActiveCellsRow(Columns, true));
            }
        }

        /// <summary>
        /// Sets the cell activities for the specified index range.
        /// No action is taken if one of the indexes is out of range.
        /// Returns false if no action is taken.
        /// </summary>
        /// <param name="startIndex">The first index.</param>
        /// <param name="endIndex">The second index.</param>
        /// <param name="activity">The cell activity.</param>
        public bool SetCellActivities(Vector2Int startIndex, Vector2Int endIndex, CellActivity activity)
        {
            if (activity == CellActivity.None || !CellIndexRangeExists(startIndex, endIndex))
                return false;

            var startRow = Mathf.Min(startIndex.x, endIndex.x);
            var endRow = Mathf.Max(startIndex.x, endIndex.x);
            var startColumn = Mathf.Min(startIndex.y, endIndex.y);
            var endColumn = Mathf.Max(startIndex.y, endIndex.y);

            for (int i = startRow; i <= endRow; i++)
            {
                for (int j = startColumn; j <= endColumn; j++)
                {
                    SetCellActivity(i, j, activity);
                }
            }

            return true;
        }

        /// <summary>
        /// Sets the cell activity for the specified index.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <param name="activity">The cell activity.</param>
        /// <exception cref="System.IndexOutOfRangeException">Raised if the specified index is out of range.</exception>
        /// <exception cref="System.NotImplementedException">Raised if the specified cell activity is not handled.</exception>
        public void SetCellActivity(int row, int column, CellActivity activity)
        {
            if (!CellIndexExists(row, column))
                throw new System.IndexOutOfRangeException($"Index out of range: ({row}, {column})");

            switch (activity)
            {
                case CellActivity.None:
                    break;
                case CellActivity.Activate:
                    ActiveCells[row].Values[column] = true;
                    break;
                case CellActivity.Deactivate:
                    ActiveCells[row].Values[column] = false;
                    break;
                case CellActivity.Toggle:
                    ActiveCells[row].Values[column] = !ActiveCells[row].Values[column];
                    break;
                default:
                    throw new System.NotImplementedException($"Unhandled cell activity: {activity}.");
            }
        }

        /// <summary>
        /// Sets the cell activity for the specified index.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <param name="activity">The cell activity.</param>
        /// <exception cref="System.IndexOutOfRangeException">Raised if the specified index is out of range.</exception>
        public void SetCellActivity(int row, int column, bool activity)
        {
            if (!CellIndexExists(row, column))
                throw new System.IndexOutOfRangeException($"Index out of range: ({row}, {column})");

            ActiveCells[row].Values[column] = activity;
        }

        /// <summary>
        /// Returns the cell activity for the specified index.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <exception cref="System.IndexOutOfRangeException">Raised if the specified index is out of range.</exception>
        public bool GetCellActivity(int row, int column)
        {
            if (!CellIndexExists(row, column))
                throw new System.IndexOutOfRangeException($"Index out of range: ({row}, {column})");

            return ActiveCells[row].Values[column];
        }

        /// <summary>
        /// Returns true if the cell index exists.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        public bool CellIndexExists(int row, int column)
        {
            return (uint)row < (uint)Rows && (uint)column < (uint)Columns;
        }

        /// <summary>
        /// Returns true if both cell indexes exist.
        /// </summary>
        /// <param name="startIndex">The first index.</param>
        /// <param name="endIndex">The second index.</param>
        public bool CellIndexRangeExists(Vector2Int startIndex, Vector2Int endIndex)
        {
            return CellIndexExists(startIndex.x, startIndex.y) && CellIndexExists(endIndex.x, endIndex.y);
        }

        /// <summary>
        /// Runs auto assign on the room's cell children. Returns the number of children.
        /// </summary>
        public int AutoAssign()
        {
            Size = Size;
            var children = GetComponentsInChildren<CellChild>();

            foreach (var child in children)
            {
                child.AutoAssign(this);
            }

            return children.Length;
        }

        /// <summary>
        /// Returns the quaternion angle perpendicular to the cell plane.
        /// </summary>
        /// <exception cref="System.ArgumentException">Raised if the room type is not handled.</exception>
        public Quaternion GetCellViewDirection()
        {
            switch (RoomType)
            {
                case RoomType.TwoDimensional:
                case RoomType.ThreeDimensionalXY:
                    return Quaternion.Euler(0, 0, 0);
                case RoomType.ThreeDimensionalXZ:
                    return Quaternion.Euler(90, 0, 0);
                default:
                    throw new System.ArgumentException($"Unhandled room type: {RoomType}.");
            }
        }

        /// <summary>
        /// Returns the center of the cell grid in grid coordinates.
        /// </summary>
        public Vector3 CenterGridPosition()
        {
            return new Vector3(Columns * CellSize.x, Rows * CellSize.y, CellSize.z) * 0.5f;
        }

        /// <summary>
        /// Returns the center of the cell grid in local coordinates.
        /// </summary>
        public Vector3 CenterLocalPosition()
        {
            return GridToLocalPosition(CenterGridPosition());
        }

        /// <summary>
        /// Returns the center of the cell grid in global coordinates.
        /// </summary>
        public Vector3 CenterGlobalPosition()
        {
            return CenterLocalPosition() + transform.position;
        }

        /// <summary>
        /// Returns the cell size in local coordinates.
        /// </summary>
        /// <exception cref="System.ArgumentException">Raised if the room type is not handled.</exception>
        public Vector3 LocalCellSize()
        {
            switch (RoomType)
            {
                case RoomType.TwoDimensional:
                case RoomType.ThreeDimensionalXY:
                    return CellSize;
                case RoomType.ThreeDimensionalXZ:
                    return new Vector3(CellSize.x, CellSize.z, CellSize.y);
                default:
                    throw new System.ArgumentException($"Unhandled room type: {RoomType}.");
            }
        }

        /// <summary>
        /// Converts the specified grid coordinate to local coordinates.
        /// </summary>
        /// <param name="gridPosition">The grid coordinate position.</param>
        /// <exception cref="System.ArgumentException">Raised if the room type is not handled.</exception>
        public Vector3 GridToLocalPosition(Vector3 gridPosition)
        {
            switch (RoomType)
            {
                case RoomType.TwoDimensional:
                case RoomType.ThreeDimensionalXY:
                    return new Vector3(gridPosition.x, -gridPosition.y, -gridPosition.z);
                case RoomType.ThreeDimensionalXZ:
                    return new Vector3(gridPosition.x, gridPosition.z, -gridPosition.y);
                default:
                    throw new System.ArgumentException($"Unhandled room type: {RoomType}.");
            }
        }

        /// <summary>
        /// Converts the specified local coordinate to grid coordinates.
        /// </summary>
        /// <param name="localPosition">The local coordinate position.</param>
        /// <exception cref="System.ArgumentException">Raised if the room type is not handled.</exception>
        public Vector3 LocalToGridPosition(Vector3 localPosition)
        {
            switch (RoomType)
            {
                case RoomType.TwoDimensional:
                case RoomType.ThreeDimensionalXY:
                    return new Vector3(localPosition.x, -localPosition.y, -localPosition.z);
                case RoomType.ThreeDimensionalXZ:
                    return new Vector3(localPosition.x, -localPosition.z, localPosition.y);
                default:
                    throw new System.ArgumentException($"Unhandled room type: {RoomType}.");
            }
        }

        /// <summary>
        /// Converts the specified global position to grid coordinates.
        /// </summary>
        /// <param name="globalPosition">The global coordinate position.</param>
        /// <exception cref="System.ArgumentException">Raised if the room type is not handled.</exception>
        public Vector3 GlobalToGridPosition(Vector3 globalPosition)
        {
            return LocalToGridPosition(globalPosition - transform.position);
        }

        /// <summary>
        /// Returns the cell center for the specified index in grid coordinates.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        public Vector3 CellCenterGridPosition(int row, int column)
        {
            return new Vector3(CellSize.x * column, CellSize.y * row, 0) + 0.5f * CellSize;
        }

        /// <summary>
        /// Returns the cell center for the specified index in global coordinates.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        public Vector3 CellCenterGlobalPosition(int row, int column)
        {
            return CellCenterLocalPosition(row, column) + transform.position;
        }

        /// <summary>
        /// Returns the cell center for the specified index in local coordinates.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        public Vector3 CellCenterLocalPosition(int row, int column)
        {
            return GridToLocalPosition(CellCenterGridPosition(row, column));
        }

        /// <summary>
        /// Converts the specified grid position to the containing cell index.
        /// If the cell index does not exist, returns (-1, -1).
        /// </summary>
        /// <param name="position">The grid position.</param>
        public Vector2Int GridPositionToCellIndex(Vector3 position)
        {
            var row = Mathf.FloorToInt(position.y / CellSize.y);
            var column = Mathf.FloorToInt(position.x / CellSize.x);

            if (CellIndexExists(row, column))
                return new Vector2Int(row, column);

            return new Vector2Int(-1, -1);
        }

        /// <summary>
        /// Converts the specified local position to the containing cell index.
        /// If the cell index does not exist, returns (-1, -1).
        /// </summary>
        /// <param name="position">The local position.</param>
        public Vector2Int LocalPositionToCellIndex(Vector3 position)
        {
            return GridPositionToCellIndex(LocalToGridPosition(position));
        }

        /// <summary>
        /// Converts the specified global position to the containing cell index.
        /// If the cell index does not exist, returns (-1, -1).
        /// </summary>
        /// <param name="position">The global position.</param>
        public Vector2Int GlobalPositionToCellIndex(Vector3 position)
        {
            return LocalPositionToCellIndex(position - transform.position);
        }

        /// <summary>
        /// Returns the Mania Map room template used by the procedural generator.
        /// </summary>
        /// <param name="id">The template ID.</param>
        /// <param name="name">The template name.</param>
        public RoomTemplate GetMMRoomTemplate(int id, string name)
        {
            var cells = GetMMCells();
            AddMMDoors(cells);
            AddMMFeatures(cells);
            var spots = GetMMCollectableSpots();
            var template = new RoomTemplate(id, name, cells, spots);
            template.Validate();
            ValidateRoomFlags();
            return template;
        }

        /// <summary>
        /// Returns the Mania Map cells used by the procedural generator.
        /// </summary>
        private Array2D<Cell> GetMMCells()
        {
            var cells = new Array2D<Cell>(Rows, Columns);

            for (int i = 0; i < cells.Rows; i++)
            {
                for (int j = 0; j < cells.Columns; j++)
                {
                    if (GetCellActivity(i, j))
                        cells[i, j] = Cell.New;
                }
            }

            return cells;
        }

        /// <summary>
        /// Adds the Mania Map doors to the cells array.
        /// </summary>
        /// <param name="cells">The cells array.</param>
        private void AddMMDoors(Array2D<Cell> cells)
        {
            foreach (var door in GetComponentsInChildren<DoorComponent>())
            {
                var cell = cells[door.Row, door.Column];
                cell.SetDoor(door.Direction, door.GetMMDoor());
            }
        }

        /// <summary>
        /// Adds the cell features to the cells array.
        /// </summary>
        /// <param name="cells">The cells array.</param>
        private void AddMMFeatures(Array2D<Cell> cells)
        {
            foreach (var feature in GetComponentsInChildren<Feature>())
            {
                var cell = cells[feature.Row, feature.Column];
                cell.AddFeature(feature.Name);
            }
        }

        /// <summary>
        /// Returns the Mania Map collectable spots by ID.
        /// </summary>
        private Dictionary<int, CollectableSpot> GetMMCollectableSpots()
        {
            var spots = GetComponentsInChildren<CollectableSpotComponent>();
            var result = new Dictionary<int, CollectableSpot>(spots.Length);

            foreach (var spot in spots)
            {
                result.Add(spot.Id, spot.GetMMCollectableSpot());
            }

            return result;
        }

        /// <summary>
        /// Validates that the room flag ID's are unique.
        /// </summary>
        /// <exception cref="DuplicateIdException">Raised if two room flags have the same ID.</exception>
        public void ValidateRoomFlags()
        {
            var set = new HashSet<int>();

            foreach (var flag in GetComponentsInChildren<RoomFlag>())
            {
                if (!set.Add(flag.Id))
                    throw new DuplicateIdException($"Duplicate room flag ID {flag.Id} for object {flag}.");
            }
        }

        /// <summary>
        /// Returns the closest active cell index for the specified global position.
        /// </summary>
        /// <param name="position">The global position.</param>
        public Vector2Int FindClosestActiveCellIndex(Vector3 position)
        {
            var fastIndex = GlobalPositionToCellIndex(position);

            if (CellIndexExists(fastIndex.x, fastIndex.y) && GetCellActivity(fastIndex.x, fastIndex.y))
                return fastIndex;

            var index = Vector2Int.zero;
            var minDistance = float.PositiveInfinity;
            var gridPosition = GlobalToGridPosition(position);

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (GetCellActivity(i, j))
                    {
                        Vector2 delta = CellCenterGridPosition(i, j) - gridPosition;
                        var distance = delta.sqrMagnitude;

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            index = new Vector2Int(i, j);
                        }
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// Returns the closest door direction based on the specified global position.
        /// </summary>
        /// <param name="row">The cell row index.</param>
        /// <param name="column">The cell column index.</param>
        /// <param name="position">The global position.</param>
        public DoorDirection FindClosestDoorDirection(int row, int column, Vector3 position)
        {
            System.Span<DoorDirection> directions = stackalloc DoorDirection[]
            {
                DoorDirection.North,
                DoorDirection.East,
                DoorDirection.South,
                DoorDirection.West,
                DoorDirection.Top,
                DoorDirection.Bottom,
            };

            System.Span<Vector3> vectors = stackalloc Vector3[]
            {
                new Vector3(0, -1, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(-1, 0, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 0, -1),
            };

            var index = 0;
            var maxDistance = float.NegativeInfinity;
            var count = RoomType == RoomType.TwoDimensional ? 4 : vectors.Length;
            var delta = GlobalToGridPosition(position) - CellCenterGridPosition(row, column);
            delta.x /= CellSize.x;
            delta.y /= CellSize.y;
            delta.z /= CellSize.z;

            for (int i = 0; i < count; i++)
            {
                var distance = Vector3.Dot(delta, vectors[i]);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    index = i;
                }
            }

            return directions[index];
        }
    }
}