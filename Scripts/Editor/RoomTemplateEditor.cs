using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// The RoomTemplate editor.
    /// </summary>
    [CustomEditor(typeof(RoomTemplate))]
    public class RoomTemplateEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            serializedObject.Update();
            var prop = serializedObject.GetIterator();
            var enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                EditorGUILayout.PropertyField(prop, true);
                enterChildren = false;
            }

            GUI.enabled = true;
            serializedObject.ApplyModifiedProperties();
        }
    }
}