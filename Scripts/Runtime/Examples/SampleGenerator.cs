using MPewsey.ManiaMapUnity.Generators;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Examples
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
        private GenerationPipeline _pipeline;
        /// <summary>
        /// The generation pipeline.
        /// </summary>
        public GenerationPipeline Pipeline { get => _pipeline; set => _pipeline = value; }

        private GameObject _container;
        /// <summary>
        /// The container where all rooms are instantiated.
        /// </summary>
        public GameObject Container { get => _container; set => _container = value; }

        /// <summary>
        /// Centers the camera on the current layout and sets its view size.
        /// </summary>
        public void CenterCamera()
        {
            //var camera = Camera.main;
            //var bounds = ManiaMapManager.Current.Layout.GetBounds();
            //var x = CellSize.x * (bounds.X + 0.5f * bounds.Width);
            //var y = -CellSize.y * (bounds.Y + 0.5f * bounds.Height);
            //camera.transform.position = new Vector3(x, y, camera.transform.position.z);
            //camera.orthographicSize = 0.5f * CellSize.y * (bounds.Height + 2);
        }
    }
}
