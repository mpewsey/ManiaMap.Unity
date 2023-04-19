using MPewsey.Common.Collections;
using MPewsey.ManiaMap.Unity.Exceptions;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A component for creating a room.
    /// </summary>
    public class RoomBehavior : MonoBehaviour
    {
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

        [Header("Cells")]

        [SerializeField]
        private Transform _cellContainer;
        /// <summary>
        /// The cell container.
        /// </summary>
        public Transform CellContainer { get => _cellContainer; set => _cellContainer = value; }

        [SerializeField]
        private CellPlane _cellPlane;
        /// <summary>
        /// The plane in which the cells reside.
        /// </summary>
        public CellPlane CellPlane { get => _cellPlane; set => _cellPlane = value; }

        [SerializeField]
        private Vector2 _cellSize = Vector2.one;
        /// <summary>
        /// The size of each cell.
        /// </summary>
        public Vector2 CellSize { get => _cellSize; set => _cellSize = Vector2.Max(value, Vector2.one); }

        [SerializeField]
        private Vector2Int _size = Vector2Int.one;
        /// <summary>
        /// The size of the room grid.
        /// </summary>
        public Vector2Int Size { get => _size; set => _size = Vector2Int.Max(value, Vector2Int.one); }

        [SerializeField]
        private UnityEvent _onInitialize;
        /// <summary>
        /// An event invoked when the room is initialized.
        /// </summary>
        public UnityEvent OnInitialize { get => _onInitialize; set => _onInitialize = value; }

        /// <summary>
        /// True if the room has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        private Layout _layout;
        /// <summary>
        /// The layout.
        /// </summary>
        public Layout Layout { get => AssertIsInitialized(_layout); private set => _layout = value; }

        private LayoutState _layoutState;
        /// <summary>
        /// The layout state.
        /// </summary>
        public LayoutState LayoutState { get => AssertIsInitialized(_layoutState); private set => _layoutState = value; }

        private Room _roomLayout;
        /// <summary>
        /// The room data.
        /// </summary>
        public Room RoomLayout { get => AssertIsInitialized(_roomLayout); private set => _roomLayout = value; }

        private RoomState _roomState;
        /// <summary>
        /// The room state.
        /// </summary>
        public RoomState RoomState { get => AssertIsInitialized(_roomState); private set => _roomState = value; }

        private IReadOnlyList<DoorConnection> _doorConnections = System.Array.Empty<DoorConnection>();
        /// <summary>
        /// The room's door connections.
        /// </summary>
        public IReadOnlyList<DoorConnection> DoorConnections { get => AssertIsInitialized(_doorConnections); private set => _doorConnections = value; }

        private void Start()
        {
            if (!IsInitialized)
            {
                enabled = false;
                throw new RoomNotInitializedException($"Room not initialized: {this}.");
            }
        }

        private void OnValidate()
        {
            Id = ManiaMapManager.AutoAssignId(Id);
            Size = Size;
            CellSize = CellSize;
        }

        private void Update()
        {
            SetCellVisibility();
        }

        /// <summary>
        /// If the room is initialized, returns the value. Otherwise, throws an exception.
        /// </summary>
        /// <param name="value">The return value.</param>
        /// <exception cref="RoomNotInitializedException">Raised if the room is not initialized.</exception>
        private T AssertIsInitialized<T>(T value)
        {
            if (!IsInitialized)
                throw new RoomNotInitializedException($"Attempting to access initialized member on uninitialized room: {this}.");

            return value;
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

            handle.Completed += x =>
            {
                var room = x.Result.GetComponent<RoomBehavior>();
                room.Initialize(layout, layoutState, roomLayout, roomState, doorConnections, position);
            };

            return handle;
        }

        /// <summary>
        /// Instantiates a room prefab and initializes it.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="prefab">The room prefab.</param>
        /// <param name="parent">The parent of the instantiated room.</param>
        /// <param name="position">The option guiding the positioning of the room.</param>
        public static RoomBehavior InstantiateRoom(Uid id, GameObject prefab, Transform parent = null,
            RoomPositionOption position = RoomPositionOption.UseManagerSetting)
        {
            var manager = ManiaMapManager.Current;
            position = manager.Settings.GetRoomPositionOption(position);
            var layout = manager.Layout;
            var layoutState = manager.LayoutState;
            var roomLayout = layout.Rooms[id];
            var roomState = layoutState.RoomStates[id];
            var doorConnections = manager.GetDoorConnections(id);

            var room = Instantiate(prefab, parent).GetComponent<RoomBehavior>();
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
            IsInitialized = true;

            if (position == RoomPositionOption.LayoutPosition)
                AssignLayoutPosition();

            OnInitialize.Invoke();
        }

        /// <summary>
        /// Sets the visibility of the cell based on the player's current position.
        /// </summary>
        private void SetCellVisibility()
        {
            var player = ManiaMapManager.Current.GetPlayer();

            if (player != null)
            {
                var index = GetCellIndex(player.transform.position);
                RoomState?.SetCellVisibility(index.x, index.y, true);
            }
        }

        /// <summary>
        /// Assigns the local position of the room based on the position in the current layout.
        /// </summary>
        private void AssignLayoutPosition()
        {
            var position = new Vector2(RoomLayout.Position.Y, -RoomLayout.Position.X) * CellSize;
            transform.localPosition = Swizzle(position);
        }

        /// <summary>
        /// Auto assigns elements to the room.
        /// </summary>
        public void AutoAssign()
        {
            Id = ManiaMapManager.AutoAssignId(Id);
            CreateCells();
            AutoAssignDoors();
            AutoAssignCollectableSpots();
        }

        /// <summary>
        /// Runs the auto assign operation on all door components that are children of the room.
        /// </summary>
        private void AutoAssignDoors()
        {
            foreach (var door in GetComponentsInChildren<DoorBehavior>())
            {
                door.AutoAssign();
            }
        }

        /// <summary>
        /// Runs the auto assign operation on all collectable spot components that are children of the room.
        /// </summary>
        private void AutoAssignCollectableSpots()
        {
            foreach (var spot in GetComponentsInChildren<CollectableSpotBehavior>())
            {
                spot.AutoAssign();
            }
        }

        /// <summary>
        /// Returns a swizzled vector for a given local vector.
        /// </summary>
        /// <param name="vector">The local vector.</param>
        /// <exception cref="ArgumentException">Raised if the cell plane is unhandled.</exception>
        public Vector3 Swizzle(Vector3 vector)
        {
            switch (CellPlane)
            {
                case CellPlane.XY:
                    return new Vector3(vector.x, vector.y, -vector.z);
                case CellPlane.XZ:
                    return new Vector3(vector.x, vector.z, vector.y);
                default:
                    throw new System.ArgumentException($"Unhandled cell plane: {CellPlane}.");
            }
        }

        /// <summary>
        /// Returns a swizzled local vector for a given global vector.
        /// </summary>
        /// <param name="vector">The global vector.</param>
        /// <exception cref="ArgumentException">Raised if the cell plane is unhandled.</exception>
        public Vector3 InverseSwizzle(Vector3 vector)
        {
            switch (CellPlane)
            {
                case CellPlane.XY:
                    return new Vector3(vector.x, vector.y, -vector.z);
                case CellPlane.XZ:
                    return new Vector3(vector.x, vector.z, vector.y);
                default:
                    throw new System.ArgumentException($"Unhandled cell plane: {CellPlane}.");
            }
        }

        /// <summary>
        /// Returns the cell index containing the specified position.
        /// </summary>
        /// <param name="position">The global position.</param>
        public Vector2Int GetCellIndex(Vector3 position)
        {
            var delta = InverseSwizzle(position - transform.position);
            var row = Mathf.FloorToInt(-delta.y / CellSize.y);
            var column = Mathf.FloorToInt(delta.x / CellSize.x);
            return new Vector2Int(row, column);
        }

        /// <summary>
        /// Returns a list of all non-empty cells.
        /// </summary>
        public List<CellBehavior> GetNonEmptyCells()
        {
            var result = new List<CellBehavior>();

            for (int i = 0; i < Size.x; i++)
            {
                for (int j = 0; j < Size.y; j++)
                {
                    var cell = GetCell(i, j);

                    if (!cell.IsEmpty)
                        result.Add(cell);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the cell at the specified index.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <exception cref="System.IndexOutOfRangeException">Raised if the index is out of range.</exception>
        public CellBehavior GetCell(int row, int column)
        {
            if (!IndexExists(row, column))
                throw new System.IndexOutOfRangeException($"Index out of range: ({row}, {column}).");
            return CellContainer.GetChild(row).GetChild(column).GetComponent<CellBehavior>();
        }

        /// <summary>
        /// Returns true if the cell index exists.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IndexExists(int row, int column)
        {
            return row < Size.x && column < Size.y && row >= 0 && column >= 0;
        }

        /// <summary>
        /// Creates the cell container if it does not exist and assigns it to the object.
        /// </summary>
        public void CreateCellContainer()
        {
            if (CellContainer == null)
            {
                CellContainer = new GameObject("<Cells>").transform;
                CellContainer.SetParent(transform);
            }
        }

        /// <summary>
        /// Creates and initializes the room cells. This method will attempt
        /// to reuse existing objects if it can. Extra cells are also destroyed.
        /// </summary>
        public void CreateCells()
        {
            CreateCellContainer();

            // Destroy extra rows.
            while (CellContainer.childCount > Size.x)
            {
                var row = CellContainer.GetChild(CellContainer.childCount - 1);
                DestroyImmediate(row.gameObject);
            }

            // Create new rows.
            while (CellContainer.childCount < Size.x)
            {
                var obj = new GameObject("<New Row>");
                obj.transform.SetParent(CellContainer);
            }

            for (int i = 0; i < CellContainer.childCount; i++)
            {
                CreateRow(i);
            }
        }

        /// <summary>
        /// Creates the row and cells for the specified index. This method will attempt
        /// to reuse existing objects if it can. Extra cells are also destroyed.
        /// </summary>
        /// <param name="row">The row index.</param>
        private void CreateRow(int row)
        {
            var container = CellContainer.GetChild(row);
            container.name = $"<Row {row}>";

            // Destroy extra cells.
            while (container.childCount > Size.y)
            {
                var cell = container.GetChild(container.childCount - 1);
                DestroyImmediate(cell.gameObject);
            }

            // Create new cells.
            while (container.childCount < Size.y)
            {
                var obj = new GameObject("<New Cell>");
                obj.transform.SetParent(container);
                obj.AddComponent<CellBehavior>();
            }

            for (int j = 0; j < container.childCount; j++)
            {
                var index = new Vector2Int(row, j);
                var cell = container.GetChild(j).GetComponent<CellBehavior>();
                cell.Initialize(this, index);
            }
        }

        /// <summary>
        /// Returns a new generation room template.
        /// </summary>
        public RoomTemplate CreateData()
        {
            AutoAssign();
            var cells = new Array2D<Cell>(Size.x, Size.y);

            for (int i = 0; i < cells.Rows; i++)
            {
                for (int j = 0; j < cells.Columns; j++)
                {
                    cells[i, j] = GetCell(i, j).CreateData();
                }
            }

            var template = new RoomTemplate(Id, Name, cells);
            template.Validate();
            ValidateRoomFlags();
            return template;
        }

        /// <summary>
        /// Validates that all room flags are unique and raises an exception if not.
        /// </summary>
        /// <exception cref="DuplicateRoomFlagIdException">Raised if any room flag ID is not unique.</exception>
        public void ValidateRoomFlags()
        {
            var set = new HashSet<int>();
            var flags = GetComponentsInChildren<RoomFlag>();

            foreach (var flag in flags)
            {
                if (!set.Add(flag.Id))
                    throw new DuplicateRoomFlagIdException($"Duplicate room flag ID {flag.Id} for object {flag}.");
            }
        }
    }
}