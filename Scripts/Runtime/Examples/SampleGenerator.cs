using MPewsey.ManiaMap.Unity.Generators;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Examples
{
    /// <summary>
    /// The layout generation controller for the sample scene.
    /// </summary>
    public class SampleGenerator : MonoBehaviour
    {
        [SerializeField]
        private Vector2 _cellSize = new Vector2(10, 10);
        /// <summary>
        /// The room cell width and height.
        /// </summary>
        public Vector2 CellSize { get => _cellSize; set => _cellSize = value; }

        [SerializeField]
        private GenerationPipeline _generator;
        /// <summary>
        /// The generation pipeline.
        /// </summary>
        public GenerationPipeline Generator { get => _generator; set => _generator = value; }

        [SerializeField]
        private RoomPrefabDatabase _roomDatabase;
        /// <summary>
        /// The room prefab database.
        /// </summary>
        public RoomPrefabDatabase RoomDatabase { get => _roomDatabase; set => _roomDatabase = value; }

        private GameObject _roomContainer;
        /// <summary>
        /// The container where all rooms are instantiated.
        /// </summary>
        public GameObject RoomContainer { get => _roomContainer; set => _roomContainer = value; }

        private void Start()
        {
            GenerateLayout();
            CenterCamera();
        }

        private void Update()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                GenerateLayout();
                CenterCamera();
            }
        }

        /// <summary>
        /// Generates a new layout.
        /// </summary>
        public void GenerateLayout()
        {
            Destroy(RoomContainer);
            RoomContainer = new GameObject("Rooms");
            var results = Generator.Generate();

            if (!results.Success)
            {
                Debug.LogError("Failed to generate layout.");
                return;
            }

            var layout = (Layout)results.Outputs["Layout"];
            var layoutState = new LayoutState(layout);
            ManiaMapManager.Current.SetLayout(layout, layoutState);
            RoomDatabase.InstantiateLayer(0, RoomContainer.transform);
        }

        /// <summary>
        /// Centers the camera on the current layout and sets its view size.
        /// </summary>
        public void CenterCamera()
        {
            var camera = Camera.main;
            var bounds = ManiaMapManager.Current.Layout.GetBounds();
            var x = CellSize.x * (bounds.X + 0.5f * bounds.Width);
            var y = -CellSize.y * (bounds.Y + 0.5f * bounds.Height);
            camera.transform.position = new Vector3(x, y, camera.transform.position.z);
            camera.orthographicSize = 0.5f * CellSize.y * (bounds.Height + 2);
        }
    }
}
