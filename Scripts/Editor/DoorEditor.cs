using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
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

        private bool MultipleTargetsSelected()
        {
            return serializedObject.targetObjects.Length > 1;
        }

        private Door GetDoor()
        {
            return (Door)serializedObject.targetObject;
        }

        private void DrawAutoAssignButton()
        {
            if (GUILayout.Button("Auto Assign"))
            {
                AutoAssign();
            }
        }

        private void DrawCellErrorBox()
        {
            if (GetDoor().Cell == null)
                EditorGUILayout.HelpBox("No cell assigned to door.", MessageType.Error, true);
        }

        private void AutoAssign()
        {
            var door = GetDoor();
            door.AutoAssign();
            Debug.Log($"<color=#00FF00><b>Auto assigned door.</b></color>");
        }

        [MenuItem("GameObject/Mania Map/Door", priority = 20)]
        [MenuItem("Mania Map/Create Door", priority = 100)]
        public static void CreateDoor()
        {
            var obj = new GameObject("Door");
            obj.AddComponent<Door>();
            obj.transform.SetParent(Selection.activeTransform);
        }
    }
}