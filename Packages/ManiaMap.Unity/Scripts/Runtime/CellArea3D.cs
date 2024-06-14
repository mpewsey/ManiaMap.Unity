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
            int cellLayer, LayerMask triggeringLayers)
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
            collider.includeLayers = triggeringLayers;
            collider.excludeLayers = ~triggeringLayers;

            return area;
        }

        private void OnTriggerEnter(Collider collision)
        {
            Room.RoomState.SetCellVisibility(Row, Column, true);
            Room.OnCellAreaEntered.Invoke(this, collision.gameObject);
        }

        private void OnTriggerExit(Collider collision)
        {
            Room.OnCellAreaExited.Invoke(this, collision.gameObject);
        }
    }
}