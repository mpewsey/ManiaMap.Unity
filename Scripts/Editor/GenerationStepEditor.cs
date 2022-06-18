using UnityEditor;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// The GenerationStep editor.
    /// </summary>
    [CustomEditor(typeof(GenerationStep), true)]
    public class GenerationStepEditor : UnityEditor.Editor
    {
        /// <summary>
        /// The long dash character.
        /// </summary>
        private const string LongDash = "\u2012";

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
            var step = GetGenerationStep();
            var names = step.InputNames();
            var label = names.Length > 0 ? string.Join(", ", names) : LongDash;
            EditorGUILayout.LabelField("Input Names", label);
        }

        /// <summary>
        /// Draws the output names label.
        /// </summary>
        private void DrawOutputNamesLabel()
        {
            var step = GetGenerationStep();
            var names = step.OutputNames();
            var label = names.Length > 0 ? string.Join(", ", names) : LongDash;
            EditorGUILayout.LabelField("Output Names", label);
        }

        /// <summary>
        /// Returns the target generation step.
        /// </summary>
        private GenerationStep GetGenerationStep()
        {
            return (GenerationStep)serializedObject.targetObject;
        }
    }
}
