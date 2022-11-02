using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// The SampleSaveSettings editor.
    /// </summary>
    [CustomEditor(typeof(SampleSaveSettings))]
    public class SampleSaveSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawSaveTemplatesButton();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Returns the target settings.
        /// </summary>
        private SampleSaveSettings GetSettings()
        {
            return (SampleSaveSettings)serializedObject.targetObject;
        }

        /// <summary>
        /// Draws the save templates button.
        /// </summary>
        private void DrawSaveTemplatesButton()
        {
            if (GUILayout.Button("Save Sample Templates"))
                GetSettings().CreateSampleTemplates();
        }
    }
}