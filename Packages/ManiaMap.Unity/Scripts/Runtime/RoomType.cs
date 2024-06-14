using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// The type of room.
    /// </summary>
    public enum RoomType
    {
        [InspectorName("2D")]
        TwoDimensional, /// A 2D room in the XY plane.
        [InspectorName("3D - XY Plane")]
        ThreeDimensionalXY, /// A 3D room in the XY plane.
        [InspectorName("3D - XZ Plane")]
        ThreeDimensionalXZ, /// A 3D room in the XZ plane.
    }
}