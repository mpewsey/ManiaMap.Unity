using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    public enum RoomType
    {
        [InspectorName("2D")]
        TwoDimensional,
        [InspectorName("3D - XY Plane")]
        ThreeDimensionalXY,
        [InspectorName("3D - XZ Plane")]
        ThreeDimensionalXZ,
    }
}