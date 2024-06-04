using UnityEditor;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The CollectableSpot editor.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CellChild))]
    public class CellChildEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawRoomErrorBox();
        }

        protected void DrawRoomErrorBox()
        {
            var child = (CellChild)serializedObject.targetObject;

            if (child.Room == null)
                EditorGUILayout.HelpBox("Room not assigned.", MessageType.Error, true);
        }
    }
}
