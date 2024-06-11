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
            if (GUILayout.Button(RoomComponentToolbar.DisplayToolbar ? "Hide Toolbar" : "Show Toolbar"))
                RoomComponentToolbar.ToggleToolbarDisplay();

            base.OnInspectorGUI();
        }
    }
}
