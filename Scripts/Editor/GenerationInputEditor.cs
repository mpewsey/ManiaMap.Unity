using UnityEditor;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// The GenerationInput editor.
    /// </summary>
    [CustomEditor(typeof(GenerationInput), true)]
    public class GenerationInputEditor : UnityEditor.Editor
    {
        /// <summary>
        /// The long dash character.
        /// </summary>
        private const string LongDash = "\u2012";

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
            var input = GetGenerationInput();
            var names = input.OutputNames();
            var label = names.Length > 0 ? string.Join(", ", names) : LongDash;
            EditorGUILayout.LabelField("Output Names", label);
        }

        /// <summary>
        /// Returns the target generation input object.
        /// </summary>
        private GenerationInput GetGenerationInput()
        {
            return (GenerationInput)serializedObject.targetObject;
        }
    }
}
