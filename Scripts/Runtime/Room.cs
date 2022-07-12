using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A component for creating a room.
    /// </summary>
    public class Room : MonoBehaviour
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
        private Plane _cellPlane;
        /// <summary>
        /// The plane in which the cells reside.
        /// </summary>
        public Plane CellPlane { get => _cellPlane; set => _cellPlane = value; }

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

        /// <summary>
        /// The room ID.
        /// </summary>
        public Uid RoomId { get; private set; } = new Uid(-1, -1, -1);

        private void OnValidate()
        {
            AutoAssignId();
            Size = Size;
            CellSize = CellSize;
        }

        /// <summary>
        /// Instantiates a room prefab and initializes it.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="prefab">The asset reference for the room prefab.</param>
        /// <param name="parent">The parent of the instantiated room.</param>
        /// <param name="position">The option guiding the positioning of the room.</param>
        public static async Task<Room> InstantiateRoomAsync(Uid id, AssetReferenceGameObject prefab, Transform parent = null, RoomPositionOption position = RoomPositionOption.Default)
        {
            var handle = InstantiateRoomHandle(id, prefab, parent, position);
            await handle.Task;
            return handle.Result.GetComponent<Room>();
        }

        /// <summary>
        /// Instantiates a room prefab and initializes it.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="prefab">The asset reference for the room prefab.</param>
        /// <param name="parent">The parent of the instantiated room.</param>
        /// <param name="position">The option guiding the positioning of the room.</param>
        public static Room InstantiateRoom(Uid id, AssetReferenceGameObject prefab, Transform parent = null, RoomPositionOption position = RoomPositionOption.Default)
        {
            var handle = InstantiateRoomHandle(id, prefab, parent, position);
            var obj = handle.WaitForCompletion();
            return obj.GetComponent<Room>();
        }

        /// <summary>
        /// Creates a new operation handle for instantiating the room prefab.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="prefab">The asset reference for the room prefab.</param>
        /// <param name="parent">The parent of the instantiated room.</param>
        /// <param name="position">The option guiding the positioning of the room.</param>
        private static AsyncOperationHandle<GameObject> InstantiateRoomHandle(Uid id, AssetReferenceGameObject prefab, Transform parent, RoomPositionOption position = RoomPositionOption.Default)
        {
            var handle = Addressables.InstantiateAsync(prefab, parent);
            handle.Completed += x => x.Result.GetComponent<Room>().Init(id, position);
            return handle;
        }

        /// <summary>
        /// Instantiates a room prefab and initializes it.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="prefab">The room prefab.</param>
        /// <param name="parent">The parent of the instantiated room.</param>
        /// <param name="position">The option guiding the positioning of the room.</param>
        public static Room InstantiateRoom(Uid id, GameObject prefab, Transform parent = null, RoomPositionOption position = RoomPositionOption.Default)
        {
            var obj = Instantiate(prefab, parent);
            var room = obj.GetComponent<Room>();
            room.Init(id, position);
            return room;
        }

        /// <summary>
        /// Initializes the room and its registered children.
        /// </summary>
        /// <param name="roomId">The room ID.</param>
        /// <param name="position">The option guiding the position of the room.</param>
        public void Init(Uid roomId, RoomPositionOption position)
        {
            RoomId = roomId;

            switch (position)
            {
                case RoomPositionOption.Default:
                    break;
                case RoomPositionOption.Layout:
                    AssignLayoutPosition();
                    break;
                default:
                    throw new System.ArgumentException($"Unhandled room position option: {position}.");
            }
        }

        /// <summary>
        /// Assigns the local position of the room based on the position in the current layout.
        /// </summary>
        private void AssignLayoutPosition()
        {
            var data = ManiaManager.Current.LayoutData;
            var room = data.Layout.Rooms[RoomId];
            var position = new Vector2(room.Position.Y, -room.Position.X) * CellSize;
            transform.localPosition = Swizzle(position);
        }

        /// <summary>
        /// Auto assigns elements to the room.
        /// </summary>
        public void AutoAssign()
        {
            AutoAssignId();
            CreateCells();
            AutoAssignDoors();
            AutoAssignCollectableSpots();
        }

        /// <summary>
        /// Runs the auto assign operation on all door components that are children of the room.
        /// </summary>
        private void AutoAssignDoors()
        {
            foreach (var door in GetComponentsInChildren<Door>())
            {
                door.AutoAssign();
            }
        }

        /// <summary>
        /// Runs the auto assign operation on all collectable spot components that are children of the room.
        /// </summary>
        private void AutoAssignCollectableSpots()
        {
            foreach (var spot in GetComponentsInChildren<CollectableSpot>())
            {
                spot.AutoAssign();
            }
        }

        /// <summary>
        /// If the ID is less than or equal to zero, assigns a random positive integer to the ID.
        /// </summary>
        private void AutoAssignId()
        {
            if (Id <= 0)
                Id = Random.Range(1, int.MaxValue);
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
                case Plane.XY:
                    return new Vector3(vector.x, vector.y, -vector.z);
                case Plane.XZ:
                    return new Vector3(vector.x, vector.z, vector.y);
                default:
                    throw new System.ArgumentException($"Unhandled cell plane: {CellPlane}.");
            }
        }

        /// <summary>
        /// Returns the cell at the specified index.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <exception cref="IndexOutOfRangeException">Raised if the index is out of range.</exception>
        public Cell GetCell(int row, int column)
        {
            if (!CellIndexExists(row, column))
                throw new System.IndexOutOfRangeException($"Index out of range: ({row}, {column}).");
            return CellContainer.GetChild(row).GetChild(column).GetComponent<Cell>();
        }

        /// <summary>
        /// Returns true if the cell index exists.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CellIndexExists(int row, int column)
        {
            return (uint)row < Size.x && (uint)column < Size.y;
        }

        /// <summary>
        /// Creates the cell container if it does not exist and assigns it to the object.
        /// </summary>
        public void CreateCellContainer()
        {
            if (CellContainer == null)
            {
                var obj = new GameObject("<Cells>");
                obj.transform.SetParent(transform);
                CellContainer = obj.transform;
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
                obj.AddComponent<Cell>();
            }

            for (int j = 0; j < container.childCount; j++)
            {
                var index = new Vector2Int(row, j);
                var cell = container.GetChild(j).GetComponent<Cell>();
                cell.Init(this, index);
            }
        }

        /// <summary>
        /// Returns a new generation room template.
        /// </summary>
        public ManiaMap.RoomTemplate GetTemplate()
        {
            AutoAssign();
            var cells = new Array2D<ManiaMap.Cell>(Size.x, Size.y);

            for (int i = 0; i < cells.Rows; i++)
            {
                for (int j = 0; j < cells.Columns; j++)
                {
                    cells[i, j] = GetCell(i, j).GetCell();
                }
            }

            var template = new ManiaMap.RoomTemplate(Id, Name, cells);
            template.Validate();
            return template;
        }

        /// <summary>
        /// The cell plane.
        /// </summary>
        public enum Plane
        {
            /// The XY plane.
            XY,
            /// The XZ plane.
            XZ,
        }
    }
}