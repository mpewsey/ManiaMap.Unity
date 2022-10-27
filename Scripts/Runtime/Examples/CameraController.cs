using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Examples
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private float _scrollSpeed = 10;
        public float ScrollSpeed { get => _scrollSpeed; set => _scrollSpeed = value; }

        [SerializeField]
        private float _zoomSpeed = 10;
        public float ZoomSpeed { get => _zoomSpeed; set => _zoomSpeed = value; }

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