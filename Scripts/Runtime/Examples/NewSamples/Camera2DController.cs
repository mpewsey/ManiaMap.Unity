using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Examples
{
    public class Camera2DController : MonoBehaviour
    {
        [SerializeField] private float _scrollSpeed = 1;
        public float ScrollSpeed { get => _scrollSpeed; set => _scrollSpeed = value; }

        [SerializeField] private float _zoomSpeed = 1;
        public float ZoomSpeed { get => _zoomSpeed; set => _zoomSpeed = value; }

        private Camera Camera { get; set; }
        private Vector3 InitialPosition { get; set; }
        private float InitialZoom { get; set; }

        private void Awake()
        {
            Camera = GetComponent<Camera>();
            InitialPosition = Camera.transform.position;
            InitialZoom = Camera.orthographicSize;
        }

        private void Update()
        {
            var mouseScroll = Input.mouseScrollDelta.y;
            Camera.orthographicSize = Mathf.Max(Camera.orthographicSize - ZoomSpeed * mouseScroll, 1);

            if (Input.GetButton("Fire2"))
            {
                var mouseDirection = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
                Camera.transform.position -= ScrollSpeed * mouseDirection;
            }
        }

        public void ResetPosition()
        {
            Camera.transform.position = InitialPosition;
            Camera.orthographicSize = InitialZoom;
        }
    }
}