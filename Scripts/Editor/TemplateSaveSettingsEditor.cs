using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    [CustomEditor(typeof(TemplateSaveSettings))]
    public class TemplateSaveSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawSaveTemplatesButton();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }

        private TemplateSaveSettings GetSettings()
        {
            return (TemplateSaveSettings)serializedObject.targetObject;
        }

        private void DrawSaveTemplatesButton()
        {
            if (GUILayout.Button("Batch Save Templates"))
                GetSettings().BatchSaveTemplates();
        }
    }
}