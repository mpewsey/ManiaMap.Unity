using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The Room editor.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RoomComponent))]
    public class RoomComponentEditor : UnityEditor.Editor
    {
        public static bool DisplayCells { get; private set; } = true;
        public static CellActivity CellEditMode { get; private set; } = CellActivity.None;

        /// <summary>
        /// Creates a Game Object with the Room component.
        /// </summary>
        [MenuItem("GameObject/Mania Map/Room", priority = 20)]
        [MenuItem("Mania Map/Create Room", priority = 100)]
        public static void CreateRoomTemplate()
        {
            var obj = new GameObject("Room");
            obj.AddComponent<RoomComponent>();
            obj.transform.SetParent(Selection.activeTransform);
        }

        private void OnEnable()
        {
            var room = (RoomComponent)serializedObject.targetObject;

            if (!room.TryGetComponent(out RoomCellEditorComponent editor))
                room.gameObject.AddComponent<RoomCellEditorComponent>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!MultipleTargetsSelected() && !TargetIsPrefabAsset())
                DrawToolbar();

            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            DisplayCells = GUILayout.Toggle(DisplayCells, GetGUIContent("Display Cells", "ManiaMap/Icons/CellDisplayIcon"), EditorStyles.toolbarButton);
            EditorGUILayout.Separator();

            for (int i = 0; i < 4; i++)
            {
                var editMode = GetEditMode(i);

                if (GUILayout.Toggle(CellEditMode == editMode, GetEditModeGUIContent(i), EditorStyles.toolbarButton))
                    CellEditMode = editMode;
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button(GetGUIContent("Auto Assign", "ManiaMap/Icons/AutoAssignIcon"), EditorStyles.toolbarButton))
                AutoAssign();

            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
        }

        private static GUIContent GetGUIContent(string tooltip, string iconPath)
        {
            var icon = Resources.Load<Texture2D>(iconPath);
            return new GUIContent(icon, tooltip);
        }

        private static GUIContent GetEditModeGUIContent(int slot)
        {
            switch (slot)
            {
                case 0:
                    return GetGUIContent("Disable Cell Editing", "ManiaMap/Icons/NoneEditModeIcon");
                case 1:
                    return GetGUIContent("Activate Cells", "ManiaMap/Icons/ActivateEditModeIcon");
                case 2:
                    return GetGUIContent("Deactivate Cells", "ManiaMap/Icons/DeactivateEditModeIcon");
                case 3:
                    return GetGUIContent("Toggle Cells", "ManiaMap/Icons/ToggleEditModeIcon");
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

        /// <summary>
        /// Returns true if multiple targets are selected.
        /// </summary>
        private bool MultipleTargetsSelected()
        {
            return serializedObject.targetObjects.Length > 1;
        }

        /// <summary>
        /// Returns true if the target object is an unopened prefab being inspected.
        /// </summary>
        private bool TargetIsPrefabAsset()
        {
            var room = (RoomComponent)serializedObject.targetObject;
            return room.gameObject.scene.name == null;
        }

        /// <summary>
        /// Runs auto assign on the room.
        /// </summary>
        private void AutoAssign()
        {
            var room = (RoomComponent)serializedObject.targetObject;
            var count = room.AutoAssign();
            EditorUtility.SetDirty(room);
            Debug.Log($"<color=#00FF00><b>Auto assigned {count} cell children.</b></color>");
        }
    }
}
