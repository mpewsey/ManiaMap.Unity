using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// An interpolatable area, useful for locating characters moving between door thresholds.
    /// </summary>
    public class DoorThreshold : MonoBehaviour
    {
        [SerializeField] private Vector3 _center;
        /// <summary>
        /// The center of the threshold area.
        /// </summary>
        public Vector3 Center { get => _center; set => _center = value; }

        [SerializeField] private Vector3 _size;
        /// <summary>
        /// The size of the threshold area.
        /// </summary>
        public Vector3 Size { get => _size; set => _size = value; }

        private void OnDrawGizmos()
        {
            var bounds = AreaBounds();
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        /// <summary>
        /// Returns the global bounds of the threshold area.
        /// </summary>
        public Bounds AreaBounds()
        {
            return new Bounds(Center + transform.position, Size);
        }

        /// <summary>
        /// Returns the interpolation parameters corresponding to the specified global position.
        /// The interpolation parameters are clamped to [0, 1].
        /// </summary>
        /// <param name="position">The global position.</param>
        public Vector3 Parameterize(Vector3 position)
        {
            var bounds = AreaBounds();
            var delta = position - bounds.min;
            var range = bounds.max - bounds.min;

            var x = range.x == 0 ? 0.5f : Mathf.Clamp01(delta.x / range.x);
            var y = range.y == 0 ? 0.5f : Mathf.Clamp01(delta.y / range.y);
            var z = range.z == 0 ? 0.5f : Mathf.Clamp01(delta.z / range.z);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Returns the global position corresponding to the specified interpolation parameters.
        /// The interpolation parameters are clamped to [0, 1].
        /// </summary>
        /// <param name="parameters">The interpolation parameters.</param>
        public Vector3 Interpolate(Vector3 parameters)
        {
            var bounds = AreaBounds();
            var min = bounds.min;
            var delta = bounds.max - bounds.min;

            var x = delta.x * Mathf.Clamp01(parameters.x) + min.x;
            var y = delta.y * Mathf.Clamp01(parameters.y) + min.y;
            var z = delta.z * Mathf.Clamp01(parameters.z) + min.z;

            return new Vector3(x, y, z);
        }
    }
}