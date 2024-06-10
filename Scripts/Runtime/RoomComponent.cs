using MPewsey.Common.Collections;
using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Exceptions;
using MPewsey.ManiaMapUnity.Exceptions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// A component for creating a room.
    /// </summary>
    public class RoomComponent : MonoBehaviour
    {
        [SerializeField]
        private RoomTemplateResource _roomTemplate;
        public RoomTemplateResource RoomTemplate { get => _roomTemplate; set => _roomTemplate = value; }

        [SerializeField]
        private string _name = "<None>";
        public string Name { get => _name; set => _name = value; }

        [SerializeField]
        private RoomType _roomType;
        public RoomType RoomType { get => _roomType; set => _roomType = value; }

        [SerializeField]
        private Vector2Int _size = Vector2Int.one;
        /// <summary>
        /// The size of the room grid.
        /// </summary>
        public Vector2Int Size { get => _size; set => SetSizeField(ref _size, value); }

        [SerializeField]
        private Vector3 _cellSize = new Vector3(1, 1, 0.001f);
        /// <summary>
        /// The size of each cell.
        /// </summary>
        public Vector3 CellSize { get => _cellSize; set => _cellSize = Vector3.Max(value, 0.001f * Vector3.one); }

        [HideInInspector]
        [SerializeField]
        private List<ActiveCellsRow> _activeCells = new List<ActiveCellsRow>();
        public List<ActiveCellsRow> ActiveCells { get => _activeCells; set => _activeCells = value; }

        [SerializeField]
        private UnityEvent _onInitialize = new UnityEvent();
        /// <summary>
        /// An event invoked when the room is initialized.
        /// </summary>
        public UnityEvent OnInitialize { get => _onInitialize; set => _onInitialize = value; }

        [SerializeField]
        private UnityEvent _onInitialized = new UnityEvent();
        public UnityEvent OnInitialized { get => _onInitialized; set => _onInitialized = value; }

        [SerializeField]
        private CellAreaTriggerEvent _onCellAreaEntered = new CellAreaTriggerEvent();
        public CellAreaTriggerEvent OnCellAreaEntered { get => _onCellAreaEntered; set => _onCellAreaEntered = value; }

        [SerializeField]
        private CellAreaTriggerEvent _onCellAreaExited = new CellAreaTriggerEvent();
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

        public int Rows { get => Size.x; set => Size = new Vector2Int(value, Size.y); }
        public int Columns { get => Size.y; set => Size = new Vector2Int(Size.x, value); }

        private void SetSizeField(ref Vector2Int field, Vector2Int value)
        {
            field = Vector2Int.Max(value, Vector2Int.one);
            SizeActiveCells();
        }

        private void Start()
        {
            if (!IsInitialized)
                throw new RoomNotInitializedException($"Room not initialized: {this}.");
        }

        private void OnValidate()
        {
            Size = Size;
            CellSize = CellSize;
        }

        private void OnDrawGizmos()
        {
            var activeFillColor = new Color(0, 0, 1, 0.3f);
            var inactiveFillColor = new Color(1, 0, 0, 0.3f);
            var lineColor = new Color(0, 0, 0);
            DrawCells(activeFillColor, lineColor, true);
            DrawCells(inactiveFillColor, lineColor, false);
        }

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

        public static AsyncOperationHandle<GameObject> InstantiateRoomAsync(Uid id, LayoutPack layoutPack,
            AssetReferenceGameObject prefab, Transform parent = null,
            bool assignLayoutPosition = false)
        {
            var roomLayout = layoutPack.Layout.Rooms[id];
            var roomState = layoutPack.LayoutState.RoomStates[id];
            var cellLayer = layoutPack.Settings.CellLayer;
            var triggeringLayer = layoutPack.Settings.TriggeringLayer;
            return InstantiateRoomAsync(prefab, parent, layoutPack, roomLayout, roomState, cellLayer, triggeringLayer, assignLayoutPosition);
        }

        public static AsyncOperationHandle<GameObject> InstantiateRoomAsync(AssetReferenceGameObject prefab, Transform parent,
            LayoutPack layoutPack, Room roomLayout, RoomState roomState,
            LayerMask cellLayer, LayerMask triggeringLayer, bool assignLayoutPosition)
        {
            var handle = prefab.InstantiateAsync(parent);

            handle.Completed += handle => handle.Result.GetComponent<RoomComponent>()
                .Initialize(layoutPack, roomLayout, roomState, cellLayer, triggeringLayer, assignLayoutPosition);

            return handle;
        }

        public static RoomComponent InstantiateRoom(Uid id, LayoutPack layoutPack, GameObject prefab, Transform parent = null,
            bool assignLayoutPosition = false)
        {
            var roomLayout = layoutPack.Layout.Rooms[id];
            var roomState = layoutPack.LayoutState.RoomStates[id];
            var doorConnections = layoutPack.GetDoorConnections(id);
            var cellLayer = layoutPack.Settings.CellLayer;
            var triggeringLayer = layoutPack.Settings.TriggeringLayer;
            return InstantiateRoom(prefab, parent, layoutPack, roomLayout, roomState, cellLayer, triggeringLayer, assignLayoutPosition);
        }

        public static RoomComponent InstantiateRoom(GameObject prefab, Transform parent,
            LayoutPack layoutPack, Room roomLayout, RoomState roomState,
            LayerMask cellLayer, LayerMask triggeringLayer, bool assignLayoutPosition)
        {
            var room = Instantiate(prefab, parent).GetComponent<RoomComponent>();
            room.Initialize(layoutPack, roomLayout, roomState, cellLayer, triggeringLayer, assignLayoutPosition);
            return room;
        }

        /// <summary>
        /// Initializes the room and its registered children.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="layout">The layout.</param>
        /// <param name="layoutState">The layout state.</param>
        /// <param name="doorConnections">A list of door connections for the room.</param>
        /// <param name="position">The option guiding the position of the room.</param>
        public bool Initialize(LayoutPack layoutPack, Room roomLayout, RoomState roomState,
            LayerMask cellLayer, LayerMask triggeringLayer, bool assignLayoutPosition)
        {
            if (IsInitialized)
                return false;

            LayoutPack = layoutPack;
            RoomLayout = roomLayout;
            RoomState = roomState;

            if (assignLayoutPosition)
                MoveToLayoutPosition();

            CreateCellAreas(cellLayer, triggeringLayer);
            IsInitialized = true;
            OnInitialize.Invoke();
            return true;
        }

        private void MoveToLayoutPosition()
        {
            var position = new Vector2(RoomLayout.Position.Y, RoomLayout.Position.X) * CellSize;
            transform.localPosition = GridToLocalPosition(position);
        }

        private void CreateCellAreas(LayerMask cellLayer, LayerMask triggeringLayer)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (GetCellActivity(i, j))
                        CellArea.InstantiateCellArea(i, j, this, cellLayer, triggeringLayer);
                }
            }
        }

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

        public void SetCellActivity(int row, int column, bool activity)
        {
            if (!CellIndexExists(row, column))
                throw new System.IndexOutOfRangeException($"Index out of range: ({row}, {column})");

            ActiveCells[row].Values[column] = activity;
        }

        public bool GetCellActivity(int row, int column)
        {
            if (!CellIndexExists(row, column))
                throw new System.IndexOutOfRangeException($"Index out of range: ({row}, {column})");

            return ActiveCells[row].Values[column];
        }

        public bool CellIndexExists(int row, int column)
        {
            return (uint)row < (uint)Rows && (uint)column < (uint)Columns;
        }

        public bool CellIndexRangeExists(Vector2Int startIndex, Vector2Int endIndex)
        {
            return CellIndexExists(startIndex.x, startIndex.y) && CellIndexExists(endIndex.x, endIndex.y);
        }

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

        public Vector3 CenterGridPosition()
        {
            return new Vector3(Columns * CellSize.x, Rows * CellSize.y, CellSize.z) * 0.5f;
        }

        public Vector3 CenterLocalPosition()
        {
            return GridToLocalPosition(CenterGridPosition());
        }

        public Vector3 CenterGlobalPosition()
        {
            return CenterLocalPosition() + transform.position;
        }

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

        public Vector3 GlobalToGridPosition(Vector3 globalPosition)
        {
            return LocalToGridPosition(globalPosition - transform.position);
        }

        public Vector3 CellCenterGridPosition(int row, int column)
        {
            return new Vector3(CellSize.x * column, CellSize.y * row, 0) + 0.5f * CellSize;
        }

        public Vector3 CellCenterGlobalPosition(int row, int column)
        {
            return CellCenterLocalPosition(row, column) + transform.position;
        }

        public Vector3 CellCenterLocalPosition(int row, int column)
        {
            return GridToLocalPosition(CellCenterGridPosition(row, column));
        }

        public Vector2Int GridPositionToCellIndex(Vector3 position)
        {
            var row = Mathf.FloorToInt(position.y / CellSize.y);
            var column = Mathf.FloorToInt(position.x / CellSize.x);
            return new Vector2Int(row, column);
        }

        public Vector2Int LocalPositionToCellIndex(Vector3 position)
        {
            return GridPositionToCellIndex(LocalToGridPosition(position));
        }

        public Vector2Int GlobalPositionToCellIndex(Vector3 position)
        {
            return LocalPositionToCellIndex(position - transform.position);
        }

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

        private void AddMMDoors(Array2D<Cell> cells)
        {
            foreach (var door in GetComponentsInChildren<DoorComponent>())
            {
                var cell = cells[door.Row, door.Column];
                cell.SetDoor(door.Direction, door.GetMMDoor());
            }
        }

        private void AddMMFeatures(Array2D<Cell> cells)
        {
            foreach (var feature in GetComponentsInChildren<Feature>())
            {
                var cell = cells[feature.Row, feature.Column];
                cell.AddFeature(feature.Name);
            }
        }

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

        public void ValidateRoomFlags()
        {
            var set = new HashSet<int>();

            foreach (var flag in GetComponentsInChildren<RoomFlag>())
            {
                if (!set.Add(flag.Id))
                    throw new DuplicateIdException($"Duplicate room flag ID {flag.Id} for object {flag}.");
            }
        }

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
                var distance = Vector2.Dot(delta, vectors[i]);

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