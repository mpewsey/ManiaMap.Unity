using UnityEngine;

namespace MPewsey.ManiaMapUnity.Examples
{
    [RequireComponent(typeof(Camera))]
    public class Camera3DController : MonoBehaviour
    {
        [SerializeField] private float _scrollSpeed = 1;
        public float ScrollSpeed { get => _scrollSpeed; set => _scrollSpeed = value; }

        [SerializeField] private float _zoomSpeed = 1;
        public float ZoomSpeed { get => _zoomSpeed; set => _zoomSpeed = value; }

        private Camera Camera { get; set; }
        private Vector3 InitialPosition { get; set; }
        private Quaternion InitialRotation { get; set; }

        private void Awake()
        {
            Camera = GetComponent<Camera>();
            InitialPosition = Camera.transform.position;
            InitialRotation = Camera.transform.rotation;
        }

        private void Update()
        {
            var direction = Vector3.zero;
            direction.z = Input.mouseScrollDelta.y * ZoomSpeed;

            if (Input.GetButton("Fire2"))
            {
                direction.x = Input.GetAxis("Mouse X") * ScrollSpeed;
                direction.y = Input.GetAxis("Mouse Y") * ScrollSpeed;
            }

            Camera.transform.position += Camera.transform.TransformDirection(direction);
        }

        public void ResetPosition()
        {
            Camera.transform.SetPositionAndRotation(InitialPosition, InitialRotation);
        }
    }
}