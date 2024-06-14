using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// A 2D trigger to detect if an object on a monitored physics layer mask enters or exits a RoomComponent cell.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class CellArea2D : CellArea
    {
        /// <summary>
        /// The attached box collider.
        /// </summary>
        public BoxCollider2D Collider { get; private set; }

        private void Awake()
        {
            Collider = GetComponent<BoxCollider2D>();
        }

        /// <summary>
        /// Instantiates a new cell area within the specified room.
        /// </summary>
        /// <param name="row">The row index of the area.</param>
        /// <param name="column">The column index of the area.</param>
        /// <param name="room">The containing room.</param>
        /// <param name="cellLayer">The physics layer assigned to the new cell.</param>
        /// <param name="triggeringLayers">The physics layer mask that the cell will detect for trigger events.</param>
        public static CellArea2D InstantiateCellArea2D(int row, int column, RoomComponent room,
            int cellLayer, LayerMask triggeringLayers)
        {
            var obj = new GameObject("Cell Area 2D");
            obj.transform.SetParent(room.transform);
            obj.transform.localPosition = room.CellCenterLocalPosition(row, column);

            var area = obj.AddComponent<CellArea2D>();
            area.Initialize(row, column, room);

            var collider = area.Collider;
            collider.gameObject.layer = cellLayer;
            collider.isTrigger = true;
            collider.size = room.CellSize;
            collider.includeLayers = triggeringLayers;
            collider.excludeLayers = ~triggeringLayers;
            collider.contactCaptureLayers = triggeringLayers;
            collider.callbackLayers = triggeringLayers;

            return area;
        }

        /// <summary>
        /// Sets the RoomState cell visibility to true and calls the OnCellAreaEntered event on the containing RoomComponent.
        /// </summary>
        /// <param name="collision">The collider triggering the event.</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Room.RoomState.SetCellVisibility(Row, Column, true);
            Room.OnCellAreaEntered.Invoke(this, collision.gameObject);
        }

        /// <summary>
        /// Calls the OnCellAreaExited event on the containing RoomComponent.
        /// </summary>
        /// <param name="collision">The collider triggering the event.</param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            Room.OnCellAreaExited.Invoke(this, collision.gameObject);
        }
    }
}