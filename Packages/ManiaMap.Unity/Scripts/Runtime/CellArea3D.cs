using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    [RequireComponent(typeof(BoxCollider))]
    public class CellArea3D : CellArea
    {
        public BoxCollider Collider { get; private set; }

        private void Awake()
        {
            Collider = GetComponent<BoxCollider>();
        }

        public static CellArea3D InstantiateCellArea3D(int row, int column, RoomComponent room,
            LayerMask cellLayer, LayerMask triggeringLayer)
        {
            var obj = new GameObject("Cell Area 3D");
            obj.transform.SetParent(room.transform);
            obj.transform.localPosition = room.CellCenterLocalPosition(row, column);

            var area = obj.AddComponent<CellArea3D>();
            area.Initialize(row, column, room);

            var collider = area.Collider;
            collider.gameObject.layer = cellLayer;
            collider.isTrigger = true;
            collider.size = room.LocalCellSize();
            collider.includeLayers = triggeringLayer;
            collider.excludeLayers = ~triggeringLayer;

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