using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Generators.Editor
{
    /// <summary>
    /// The GenerationInput editor.
    /// </summary>
    [CustomEditor(typeof(GenerationInput), true)]
    public class GenerationInputEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            DrawOutputNamesLabel();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws the output names label. If no output names are defined in the input,
        /// a long dash is displayed.
        /// </summary>
        private void DrawOutputNamesLabel()
        {
            var input = (GenerationInput)serializedObject.targetObject;
            var names = input.OutputNames();
            var label = names.Length > 0 ? string.Join(", ", names) : "<N/A>";

            GUI.enabled = false;
            EditorGUILayout.LabelField("Output Names", label);
            GUI.enabled = true;
        }
    }
}
