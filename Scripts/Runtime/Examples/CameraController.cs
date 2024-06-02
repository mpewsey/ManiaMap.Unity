using UnityEngine;

namespace MPewsey.ManiaMapUnity.Examples
{
    /// <summary>
    /// A component that moves that attached camera based on the user's input.
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

        [SerializeField]
        private KeyCode _zoomInKey = KeyCode.Alpha2;
        /// <summary>
        /// The zoom in key.
        /// </summary>
        public KeyCode ZoomInKey { get => _zoomInKey; set => _zoomInKey = value; }

        [SerializeField]
        private KeyCode _zoomOutKey = KeyCode.Alpha1;
        /// <summary>
        /// The zoom out key.
        /// </summary>
        public KeyCode ZoomOutKey { get => _zoomOutKey; set => _zoomOutKey = value; }

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
            var zoom = Input.GetKey(ZoomOutKey) ? 1 : Input.GetKey(ZoomInKey) ? -1 : 0;
            Camera.transform.Translate(ScrollSpeed * Time.deltaTime * move);
            Camera.orthographicSize += ZoomSpeed * Time.deltaTime * zoom;
        }
    }
}