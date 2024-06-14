using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CellArea2D : CellArea
    {
        public BoxCollider2D Collider { get; private set; }

        private void Awake()
        {
            Collider = GetComponent<BoxCollider2D>();
        }

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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Room.RoomState.SetCellVisibility(Row, Column, true);
            Room.OnCellAreaEntered.Invoke(this, collision.gameObject);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Room.OnCellAreaExited.Invoke(this, collision.gameObject);
        }
    }
}