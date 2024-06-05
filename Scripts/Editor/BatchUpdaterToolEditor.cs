using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The TemplateSaveSettings editor.
    /// </summary>
    [CustomEditor(typeof(BatchUpdaterTool))]
    public class BatchUpdaterToolEditor : UnityEditor.Editor
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
        private BatchUpdaterTool GetSettings()
        {
            return (BatchUpdaterTool)serializedObject.targetObject;
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