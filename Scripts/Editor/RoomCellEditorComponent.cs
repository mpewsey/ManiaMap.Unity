using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    [ExecuteAlways]
    [RequireComponent(typeof(RoomComponent))]
    public class RoomCellEditorComponent : MonoBehaviour
    {
        private RoomComponent Room { get; set; }
        private bool MouseButtonPressed { get; set; }
        private Vector2 MouseButtonDownPosition { get; set; }

        private void Awake()
        {
            Room = GetComponent<RoomComponent>();
            hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
        }

        private void Update()
        {
            Debug.Log("Update");

            if (!RoomComponentEditor.DisplayCells || RoomComponentEditor.CellEditMode == CellActivity.None)
                return;

            if (Input.GetMouseButton(0))
            {
                if (!MouseButtonPressed)
                    MouseButtonDownPosition = Vector2.zero;

                MouseButtonPressed = true;
                return;
            }

            if (MouseButtonPressed)
            {
                var startIndex = Room.GlobalPositionToCellIndex(MouseButtonDownPosition);
                var endIndex = Room.GlobalPositionToCellIndex(Vector2.zero);

                if (Room.SetCellActivities(startIndex, endIndex, RoomComponentEditor.CellEditMode))
                    EditorUtility.SetDirty(Room);
            }

            MouseButtonPressed = false;
        }
    }
}
