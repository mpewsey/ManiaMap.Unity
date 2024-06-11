using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    [InitializeOnLoad]
    public class RoomComponentToolbar : UnityEditor.Editor
    {
        private const string NoneEditModeIcon = "Packages/com.mpewsey.maniamap.unity/Icons/NoneEditModeIcon.png";
        private const string ActivateEditModeIcon = "Packages/com.mpewsey.maniamap.unity/Icons/ActivateEditModeIcon.png";
        private const string DeactivateEditModeIcon = "Packages/com.mpewsey.maniamap.unity/Icons/DeactivateEditModeIcon.png";
        private const string ToggleEditModeIcon = "Packages/com.mpewsey.maniamap.unity/Icons/ToggleEditModeIcon.png";
        private const string AutoAssignIcon = "Packages/com.mpewsey.maniamap.unity/Icons/AutoAssignIcon.png";
        private const string CellDisplayIcon = "Packages/com.mpewsey.maniamap.unity/Icons/CellDisplayIcon.png";
        private const string OrientViewIcon = "Packages/com.mpewsey.maniamap.unity/Icons/OrientViewIcon.png";
        private const int LeftMouseButton = 0;

        private static RoomComponent Room { get; set; }
        private static CellActivity CellEditMode { get; set; }
        public static bool DisplayToolbar { get; private set; } = true;
        private static bool DisplayCells { get; set; }
        private static bool MouseButtonPressed { get; set; }
        private static Vector2Int MouseButtonDownIndex { get; set; }

        static RoomComponentToolbar()
        {
            SceneView.duringSceneGui += OnDuringSceneGui;
            SetGizmoEnabled(typeof(RoomComponent), DisplayCells);
        }

        public static void ToggleToolbarDisplay()
        {
            DisplayToolbar = !DisplayToolbar;
            SceneView.RepaintAll();
        }

        private static void OnDuringSceneGui(SceneView sceneView)
        {
            PollSelectedRoom();

            if (DisplayToolbar && Room != null)
                DrawToolbar(sceneView);

            ProcessEditCells(sceneView);
        }

        private static Vector3 GetMousePosition(SceneView sceneView)
        {
            var position = sceneView.camera.ScreenToWorldPoint(Event.current.mousePosition);
            position.y = 2 * sceneView.camera.transform.position.y - position.y;
            position.z = 2 * sceneView.camera.transform.position.z - position.z;
            return position;
        }

        private static void ProcessEditCells(SceneView sceneView)
        {
            if (!DisplayToolbar || !DisplayCells || CellEditMode == CellActivity.None || Room == null || !Event.current.isMouse)
                return;

            if (Event.current.type == EventType.MouseDown && Event.current.button == LeftMouseButton)
            {
                if (!MouseButtonPressed)
                    MouseButtonDownIndex = Room.GlobalPositionToCellIndex(GetMousePosition(sceneView));

                MouseButtonPressed = true;
                Event.current.Use();
                return;
            }

            if (MouseButtonPressed)
            {
                var startIndex = MouseButtonDownIndex;
                var endIndex = Room.GlobalPositionToCellIndex(GetMousePosition(sceneView));

                if (Room.SetCellActivities(startIndex, endIndex, CellEditMode))
                    EditorUtility.SetDirty(Room);
            }

            MouseButtonPressed = false;
        }

        private static void PollSelectedRoom()
        {
            var selection = Selection.activeTransform;

            if (selection == null)
                return;

            var room = selection.GetComponentInParent<RoomComponent>();

            if (room != null)
                Room = room;
        }

        private static void SetGizmoEnabled(System.Type type, bool enabled)
        {
            GizmoUtility.SetGizmoEnabled(type, enabled);
            GizmoUtility.SetGizmoEnabled(type, enabled, false);
        }

        private static void DrawToolbar(SceneView sceneView)
        {
            Handles.BeginGUI();
            // EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            DisplayCells = GUILayout.Toggle(DisplayCells, GetGUIContent("Display Cells", CellDisplayIcon), EditorStyles.toolbarButton);
            SetGizmoEnabled(typeof(RoomComponent), DisplayCells);

            if (GUILayout.Button(GetGUIContent("Orient View", OrientViewIcon), EditorStyles.toolbarButton))
                OrientView(sceneView);

            EditorGUILayout.Separator();

            for (int i = 0; i < 4; i++)
            {
                var editMode = GetEditMode(i);

                if (GUILayout.Toggle(CellEditMode == editMode, GetEditModeGUIContent(i), EditorStyles.toolbarButton))
                    CellEditMode = editMode;
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button(GetGUIContent("Auto Assign", AutoAssignIcon), EditorStyles.toolbarButton))
                BatchUpdaterTool.AutoAssign(Room);

            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            Handles.EndGUI();
        }

        private static GUIContent GetGUIContent(string tooltip, string iconPath)
        {
            var icon = (Texture2D)EditorGUIUtility.Load(iconPath);
            return new GUIContent(icon, tooltip);
        }

        private static GUIContent GetEditModeGUIContent(int slot)
        {
            switch (slot)
            {
                case 0:
                    return GetGUIContent("Disable Cell Editing", NoneEditModeIcon);
                case 1:
                    return GetGUIContent("Activate Cells", ActivateEditModeIcon);
                case 2:
                    return GetGUIContent("Deactivate Cells", DeactivateEditModeIcon);
                case 3:
                    return GetGUIContent("Toggle Cells", ToggleEditModeIcon);
                default:
                    throw new System.NotImplementedException($"Unhandled slot: {slot}.");
            }
        }

        private static CellActivity GetEditMode(int slot)
        {
            switch (slot)
            {
                case 0:
                    return CellActivity.None;
                case 1:
                    return CellActivity.Activate;
                case 2:
                    return CellActivity.Deactivate;
                case 3:
                    return CellActivity.Toggle;
                default:
                    throw new System.NotImplementedException($"Unhandled slot: {slot}.");
            }
        }

        private static void OrientView(SceneView sceneView)
        {
            sceneView.orthographic = true;
            sceneView.LookAt(Room.CenterGlobalPosition(), Room.GetCellViewDirection());
        }
    }
}
