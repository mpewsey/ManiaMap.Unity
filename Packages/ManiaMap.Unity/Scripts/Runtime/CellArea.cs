using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// The base class for cell trigger areas.
    /// </summary>
    public abstract class CellArea : MonoBehaviour
    {
        /// <summary>
        /// The row index corresponding to the area.
        /// </summary>
        public int Row { get; protected set; }

        // <summary>
        /// The column index corresponding to the area.
        /// </summary>
        public int Column { get; protected set; }

        /// <summary>
        /// The containing room.
        /// </summary>
        public RoomComponent Room { get; protected set; }

        /// <summary>
        /// Instantiates a new cell area within the specified room.
        /// </summary>
        /// <param name="row">The row index of the area.</param>
        /// <param name="column">The column index of the area.</param>
        /// <param name="room">The containing room.</param>
        /// <param name="cellLayer">The physics layer assigned to the new cell.</param>
        /// <param name="triggeringLayers">The physics layer mask that the cell will detect for trigger events.</param>
        /// <exception cref="System.NotImplementedException">Raised if the room's assigned type is not handled.</exception>
        public static CellArea InstantiateCellArea(int row, int column, RoomComponent room,
            int cellLayer, LayerMask triggeringLayers)
        {
            switch (room.RoomType)
            {
                case RoomType.TwoDimensional:
                    return CellArea2D.InstantiateCellArea2D(row, column, room, cellLayer, triggeringLayers);
                case RoomType.ThreeDimensionalXY:
                case RoomType.ThreeDimensionalXZ:
                    return CellArea3D.InstantiateCellArea3D(row, column, room, cellLayer, triggeringLayers);
                default:
                    throw new System.NotImplementedException($"Unhandled room type: {room.RoomType}.");
            }
        }

        /// <summary>
        /// Initializes the base properties for the area.
        /// </summary>
        /// <param name="row">The row index of the area.</param>
        /// <param name="column">The column index of the area.</param>
        /// <param name="room">The containing room.</param>
        protected void Initialize(int row, int column, RoomComponent room)
        {
            Row = row;
            Column = column;
            Room = room;
        }
    }
}