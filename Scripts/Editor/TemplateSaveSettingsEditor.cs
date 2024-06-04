using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The TemplateSaveSettings editor.
    /// </summary>
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

        /// <summary>
        /// Returns the target settings.
        /// </summary>
        private TemplateSaveSettings GetSettings()
        {
            return (TemplateSaveSettings)serializedObject.targetObject;
        }

        /// <summary>
        /// Draws the save templates button.
        /// </summary>
        private void DrawSaveTemplatesButton()
        {
            if (GUILayout.Button("Batch Save Templates"))
                GetSettings().BatchSaveTemplates();
        }
    }
}