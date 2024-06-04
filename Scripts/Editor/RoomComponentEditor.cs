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

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!MultipleTargetsSelected() && !TargetIsPrefabAsset())
            {
                if (GUILayout.Button("Auto Assign"))
                    AutoAssign();
            }

            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
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
