using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
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