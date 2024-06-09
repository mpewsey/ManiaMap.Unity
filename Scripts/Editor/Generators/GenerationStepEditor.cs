using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Generators.Editor
{
    /// <summary>
    /// The GenerationStep editor.
    /// </summary>
    [CustomEditor(typeof(GenerationStep), true)]
    public class GenerationStepEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            DrawInputNamesLabel();
            DrawOutputNamesLabel();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws the input names label.
        /// </summary>
        private void DrawInputNamesLabel()
        {
            var step = (GenerationStep)serializedObject.targetObject;
            var names = step.InputNames();
            var label = names.Length > 0 ? string.Join(", ", names) : "<N/A>";

            GUI.enabled = false;
            EditorGUILayout.LabelField("Input Names", label);
            GUI.enabled = true;
        }

        /// <summary>
        /// Draws the output names label.
        /// </summary>
        private void DrawOutputNamesLabel()
        {
            var step = (GenerationStep)serializedObject.targetObject;
            var names = step.OutputNames();
            var label = names.Length > 0 ? string.Join(", ", names) : "<N/A>";

            GUI.enabled = false;
            EditorGUILayout.LabelField("Output Names", label);
            GUI.enabled = true;
        }
    }
}
