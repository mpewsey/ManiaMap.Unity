using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The Room editor.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RoomBehavior))]
    public class RoomBehaviorEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Creates a Game Object with the Room component.
        /// </summary>
        [MenuItem("GameObject/Mania Map/Room", priority = 20)]
        [MenuItem("Mania Map/Create Room", priority = 100)]
        public static void CreateRoomTemplate()
        {
            var obj = new GameObject("Room");
            var template = obj.AddComponent<RoomBehavior>();
            template.CreateCells();
            obj.transform.SetParent(Selection.activeTransform);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawUpdateRoomButton();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Returns the target room.
        /// </summary>
        private RoomBehavior GetRoom()
        {
            return (RoomBehavior)serializedObject.targetObject;
        }

        /// <summary>
        /// True if multiple targets are not selected, and it is open.
        /// </summary>
        private bool ButtonsEnabled()
        {
            return !MultipleTargetsSelected() && !TargetIsPrefabAsset();
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
            return GetRoom().gameObject.scene.name == null;
        }

        /// <summary>
        /// Draws the update room button.
        /// </summary>
        private void DrawUpdateRoomButton()
        {
            if (!ButtonsEnabled())
                return;

            if (GUILayout.Button("Update Room"))
                UpdateRoom();
        }

        /// <summary>
        /// Updates the room cells.
        /// </summary>
        private void UpdateRoom()
        {
            var room = GetRoom();
            room.AutoAssign();
            EditorUtility.SetDirty(room);
            Log.Success("Room updated.");
        }
    }
}
