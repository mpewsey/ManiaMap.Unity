using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// An interpolatable area, useful for locating characters moving between door thresholds.
    /// </summary>
    public class DoorThreshold : MonoBehaviour
    {
        [SerializeField] private Vector3 _size = Vector3.one;
        /// <summary>
        /// The size of the threshold area.
        /// </summary>
        public Vector3 Size { get => _size; set => _size = Vector3.Max(value, Vector3.zero); }

        private void OnValidate()
        {
            Size = Size;
        }

        private void OnDrawGizmos()
        {
            var bounds = GetAABB();
            var lineColor = Color.yellow;
            var fillColor = new Color(lineColor.r, lineColor.g, lineColor.b, 0.2f);

            Gizmos.color = fillColor;
            Gizmos.DrawCube(bounds.center, bounds.size);
            Gizmos.color = lineColor;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        /// <summary>
        /// Returns the axis aligned bounding box for the threshold area.
        /// </summary>
        public Bounds GetAABB()
        {
            var size = 0.5f * Size;
            var min = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            var max = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

            System.Span<Vector3> corners = stackalloc Vector3[]
            {
                new Vector3(-1, -1, -1),
                new Vector3(1, -1, -1),
                new Vector3(1, 1, -1),
                new Vector3(-1, 1, -1),
                new Vector3(-1, -1, 1),
                new Vector3(1, -1, 1),
                new Vector3(1, 1, 1),
                new Vector3(-1, 1, 1),
            };

            for (int i = 0; i < corners.Length; i++)
            {
                var corner = corners[i];
                corners[i] = new Vector3(corner.x * size.x, corner.y * size.y, corner.z * size.z);
            }

            transform.TransformVectors(corners);

            foreach (var corner in corners)
            {
                min = Vector3.Min(min, corner);
                max = Vector3.Max(max, corner);
            }

            return new Bounds(Vector3.Lerp(min, max, 0.5f) + transform.position, max - min);
        }

        /// <summary>
        /// Returns the interpolation parameters corresponding to the specified global position.
        /// The interpolation parameters are clamped to [0, 1].
        /// </summary>
        /// <param name="position">The global position.</param>
        public Vector3 ParameterizePosition(Vector3 position)
        {
            var bounds = GetAABB();
            var size = bounds.size;
            var topLeft = bounds.center - 0.5f * size;
            var delta = position - topLeft;
            var x = size.x > 0 ? Mathf.Clamp(delta.x / size.x, 0, 1) : 0.5f;
            var y = size.y > 0 ? Mathf.Clamp(delta.y / size.y, 0, 1) : 0.5f;
            var z = size.z > 0 ? Mathf.Clamp(delta.z / size.z, 0, 1) : 0.5f;
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Returns the global position corresponding to the specified interpolation parameters.
        /// The interpolation parameters are clamped to [0, 1].
        /// </summary>
        /// <param name="parameters">The interpolation parameters.</param>
        public Vector3 InterpolatePosition(Vector3 parameters)
        {
            var bounds = GetAABB();
            var size = bounds.size;
            var topLeft = bounds.center - 0.5f * size;
            var bottomRight = topLeft + size;

            var x = Mathf.Lerp(topLeft.x, bottomRight.x, Mathf.Clamp(parameters.x, 0, 1));
            var y = Mathf.Lerp(topLeft.y, bottomRight.y, Mathf.Clamp(parameters.y, 0, 1));
            var z = Mathf.Lerp(topLeft.z, bottomRight.z, Mathf.Clamp(parameters.z, 0, 1));

            return new Vector3(x, y, z);
        }
    }
}