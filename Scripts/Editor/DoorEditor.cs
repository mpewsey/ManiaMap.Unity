using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// The Door editor.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Door))]
    public class DoorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!MultipleTargetsSelected() && !TargetIsPrefabAsset())
            {
                DrawAutoAssignButton();
            }

            DrawDefaultInspector();
            DrawCellErrorBox();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Returns true if the target object is an unopened prefab being inspected.
        /// </summary>
        private bool TargetIsPrefabAsset()
        {
            return GetDoor().gameObject.scene.name == null;
        }

        /// <summary>
        /// Returns true if multiple target objects are selected.
        /// </summary>
        private bool MultipleTargetsSelected()
        {
            return serializedObject.targetObjects.Length > 1;
        }

        /// <summary>
        /// Returns the target door.
        /// </summary>
        private Door GetDoor()
        {
            return (Door)serializedObject.targetObject;
        }

        /// <summary>
        /// Draws the auto assign button.
        /// </summary>
        private void DrawAutoAssignButton()
        {
            if (GUILayout.Button("Auto Assign"))
            {
                AutoAssign();
            }
        }

        /// <summary>
        /// Draws an error box if a cell is not assigned to the door.
        /// </summary>
        private void DrawCellErrorBox()
        {
            if (GetDoor().Cell == null)
                EditorGUILayout.HelpBox("No cell assigned to door.", MessageType.Error, true);
        }

        /// <summary>
        /// Auto assigns elements to the door.
        /// </summary>
        private void AutoAssign()
        {
            var door = GetDoor();
            door.AutoAssign();
            Debug.Log($"<color=#00FF00><b>Auto assigned door.</b></color>");
        }

        /// <summary>
        /// Creates a new door Game Object.
        /// </summary>
        [MenuItem("GameObject/Mania Map/Door", priority = 20)]
        [MenuItem("Mania Map/Create Door", priority = 100)]
        public static void CreateDoor()
        {
            var obj = new GameObject("Door");
            obj.transform.SetParent(Selection.activeTransform);
            obj.AddComponent<Door>();
        }
    }
}