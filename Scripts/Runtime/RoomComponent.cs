using MPewsey.Common.Collections;
using MPewsey.ManiaMap;
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
        private RoomTemplateObject _roomTemplate;
        public RoomTemplateObject RoomTemplate { get => _roomTemplate; set => _roomTemplate = value; }

        [SerializeField]
        private int _id;
        /// <summary>
        /// The unique template ID.
        /// </summary>
        public int Id { get => _id; set => _id = value; }

        [SerializeField]
        private string _name = "<None>";
        /// <summary>
        /// The template name.
        /// </summary>
        public string Name { get => _name; set => _name = value; }

        [SerializeField]
        private CellColliderType _cellColliderType;
        public CellColliderType CellCollider { get => _cellColliderType; set => _cellColliderType = value; }

        [SerializeField]
        private CellPlane _cellPlane;
        /// <summary>
        /// The plane in which the cells reside.
        /// </summary>
        public CellPlane CellPlane { get => _cellPlane; set => _cellPlane = value; }

        [SerializeField]
        private Vector2Int _size = Vector2Int.one;
        /// <summary>
        /// The size of the room grid.
        /// </summary>
        public Vector2Int Size { get => _size; set => SetSizeField(ref _size, value); }

        [SerializeField]
        private Vector3 _cellSize = new Vector3(1, 1, 0);
        /// <summary>
        /// The size of each cell.
        /// </summary>
        public Vector3 CellSize { get => _cellSize; set => _cellSize = Vector3.Max(value, new Vector3(0.001f, 0.001f, 0)); }

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

        /// <summary>
        /// True if the room has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// The layout.
        /// </summary>
        public Layout Layout { get; private set; }

        /// <summary>
        /// The layout state.
        /// </summary>
        public LayoutState LayoutState { get; private set; }

        /// <summary>
        /// The room data.
        /// </summary>
        public Room RoomLayout { get; private set; }

        /// <summary>
        /// The room state.
        /// </summary>
        public RoomState RoomState { get; private set; }

        /// <summary>
        /// The room's door connections.
        /// </summary>
        public IReadOnlyList<DoorConnection> DoorConnections { get; private set; } = System.Array.Empty<DoorConnection>();

        public int Rows { get => Size.x; set => Size = new Vector2Int(value, Size.y); }
        public int Columns { get => Size.y; set => Size = new Vector2Int(Size.x, value); }

        private void SetSizeField(ref Vector2Int field, Vector2Int value)
        {
            field = Vector2Int.Max(value, Vector2Int.one);
            SizeActiveCells();
        }

        private void Start()
        {
            if (!IsInitialized && Application.isPlaying)
                throw new RoomNotInitializedException($"Room not initialized: {this}.");
        }

        private void OnValidate()
        {
            Id = ManiaMapManager.AutoAssignId(Id);
            Size = Size;
            CellSize = CellSize;
        }

        private void OnDrawGizmos()
        {
            var activeFillColor = new Color(0, 0, 1, 0.1f);
            var inactiveFillColor = new Color(1, 0, 0, 0.1f);
            var lineColor = new Color(0.29f, 0.29f, 0.29f);
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

        /// <summary>
        /// Instantiates a room prefab and initializes it.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="prefab">The asset reference for the room prefab.</param>
        /// <param name="parent">The parent of the instantiated room.</param>
        /// <param name="position">The option guiding the positioning of the room.</param>
        public static AsyncOperationHandle<GameObject> InstantiateRoomAsync(Uid id,
            AssetReferenceGameObject prefab, Transform parent = null,
            RoomPositionOption position = RoomPositionOption.UseManagerSetting)
        {
            var manager = ManiaMapManager.Current;
            position = manager.Settings.GetRoomPositionOption(position);
            var layout = manager.Layout;
            var layoutState = manager.LayoutState;
            var roomLayout = layout.Rooms[id];
            var roomState = layoutState.RoomStates[id];
            var doorConnections = manager.GetDoorConnections(id);
            var handle = prefab.InstantiateAsync(parent);

            handle.Completed += handle => handle.Result.GetComponent<RoomComponent>()
                .Initialize(layout, layoutState, roomLayout, roomState, doorConnections, position);

            return handle;
        }

        /// <summary>
        /// Instantiates a room prefab and initializes it.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="prefab">The room prefab.</param>
        /// <param name="parent">The parent of the instantiated room.</param>
        /// <param name="position">The option guiding the positioning of the room.</param>
        public static RoomComponent InstantiateRoom(Uid id, GameObject prefab, Transform parent = null,
            RoomPositionOption position = RoomPositionOption.UseManagerSetting)
        {
            var manager = ManiaMapManager.Current;
            position = manager.Settings.GetRoomPositionOption(position);
            var layout = manager.Layout;
            var layoutState = manager.LayoutState;
            var roomLayout = layout.Rooms[id];
            var roomState = layoutState.RoomStates[id];
            var doorConnections = manager.GetDoorConnections(id);

            var room = Instantiate(prefab, parent).GetComponent<RoomComponent>();
            room.Initialize(layout, layoutState, roomLayout, roomState, doorConnections, position);
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
        public void Initialize(Layout layout, LayoutState layoutState,
            Room roomLayout, RoomState roomState, IReadOnlyList<DoorConnection> doorConnections,
            RoomPositionOption position)
        {
            Layout = layout;
            LayoutState = layoutState;
            RoomLayout = roomLayout;
            RoomState = roomState;
            DoorConnections = doorConnections;

            if (position == RoomPositionOption.LayoutPosition)
                MoveToLayoutPosition();

            IsInitialized = true;
            OnInitialize.Invoke();
        }

        /// <summary>
        /// Assigns the local position of the room based on the position in the current layout.
        /// </summary>
        private void MoveToLayoutPosition()
        {
            var position = new Vector2(RoomLayout.Position.Y, RoomLayout.Position.X) * CellSize;
            transform.localPosition = GridToLocalPosition(position);
        }

        /// <summary>
        /// Auto assigns elements to the room.
        /// </summary>
        public int AutoAssign()
        {
            SizeActiveCells();
            Id = ManiaMapManager.AutoAssignId(Id);
            var children = GetComponentsInChildren<CellChild>();

            foreach (var child in children)
            {
                child.AutoAssign(this);
            }

            return children.Length;
        }

        public Vector3 GridToLocalPosition(Vector3 vector)
        {
            switch (CellPlane)
            {
                case CellPlane.XY:
                    return new Vector3(vector.x, -vector.y, -vector.z);
                case CellPlane.XZ:
                    return new Vector3(vector.x, vector.z, -vector.y);
                default:
                    throw new System.ArgumentException($"Unhandled cell plane: {CellPlane}.");
            }
        }

        public Vector3 LocalToGridPosition(Vector3 vector)
        {
            switch (CellPlane)
            {
                case CellPlane.XY:
                    return new Vector3(vector.x, -vector.y, -vector.z);
                case CellPlane.XZ:
                    return new Vector3(vector.x, -vector.z, vector.y);
                default:
                    throw new System.ArgumentException($"Unhandled cell plane: {CellPlane}.");
            }
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

        /// <summary>
        /// Returns a new generation room template.
        /// </summary>
        public RoomTemplate GetMMRoomTemplate()
        {
            AutoAssign();
            var cells = GetMMCells();
            AddMMDoors(cells);
            AddMMFeatures(cells);
            var spots = GetMMCollectableSpots();
            var template = new RoomTemplate(Id, Name, cells, spots);
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
                    if (CellIsActive(i, j))
                        cells[i, j] = Cell.New;
                }
            }

            return cells;
        }

        private void AddMMDoors(Array2D<Cell> cells)
        {
            foreach (var door in GetComponentsInChildren<DoorBehavior>())
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

        /// <summary>
        /// Returns a new dictionary of generation data collectable spots for the room.
        /// </summary>
        private Dictionary<int, CollectableSpot> GetMMCollectableSpots()
        {
            var result = new Dictionary<int, CollectableSpot>();
            var spots = GetComponentsInChildren<CollectableSpotBehavior>();

            foreach (var spot in spots)
            {
                result.Add(spot.Id, spot.GetMMCollectableSpot());
            }

            return result;
        }

        /// <summary>
        /// Validates that all room flags are unique and raises an exception if not.
        /// </summary>
        /// <exception cref="DuplicateRoomFlagIdException">Raised if any room flag ID is not unique.</exception>
        public void ValidateRoomFlags()
        {
            var set = new HashSet<int>();

            foreach (var flag in GetComponentsInChildren<RoomFlag>())
            {
                if (!set.Add(flag.Id))
                    throw new DuplicateRoomFlagIdException($"Duplicate room flag ID {flag.Id} for object {flag}.");
            }
        }

        private void SizeActiveCells()
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

        public void SetCellActivity(int row, int column, bool activity)
        {
            if (!CellIndexExists(row, column))
                throw new System.IndexOutOfRangeException($"Index out of range: ({row}, {column})");

            ActiveCells[row].Values[column] = activity;
        }

        public bool CellIsActive(int row, int column)
        {
            if (!CellIndexExists(row, column))
                throw new System.IndexOutOfRangeException($"Index out of range: ({row}, {column})");

            return ActiveCells[row].Values[column];
        }

        public bool CellIndexExists(int row, int column)
        {
            return (uint)row < (uint)Rows && (uint)column < (uint)Columns;
        }
    }
}