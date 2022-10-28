using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Examples
{
    /// <summary>
    /// A component that moves that attached camera based on the user's input.
    /// Zoom out = 1 Key. Zoom in = 2 Key. Arrow keys move the camera.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private float _scrollSpeed = 10;
        /// <summary>
        /// The camera scroll speed.
        /// </summary>
        public float ScrollSpeed { get => _scrollSpeed; set => _scrollSpeed = value; }

        [SerializeField]
        private float _zoomSpeed = 10;
        /// <summary>
        /// The camera zoom speed.
        /// </summary>
        public float ZoomSpeed { get => _zoomSpeed; set => _zoomSpeed = value; }

        /// <summary>
        /// The attached camera component.
        /// </summary>
        public Camera Camera { get; private set; }

        private void Awake()
        {
            Camera = GetComponent<Camera>();
        }

        private void Update()
        {
            var move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Camera.transform.Translate(ScrollSpeed * Time.deltaTime * move);

            var zoom = Input.GetKey("1") ? 1 : Input.GetKey("2") ? -1 : 0;
            Camera.orthographicSize += ZoomSpeed * Time.deltaTime * zoom;
        }
    }
}