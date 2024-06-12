using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    public abstract class CellArea : MonoBehaviour
    {
        public int Row { get; protected set; }
        public int Column { get; protected set; }
        public RoomComponent Room { get; protected set; }

        public static CellArea InstantiateCellArea(int row, int column, RoomComponent room,
            LayerMask cellLayer, LayerMask triggeringLayers)
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

        protected void Initialize(int row, int column, RoomComponent room)
        {
            Row = row;
            Column = column;
            Room = room;
        }
    }
}