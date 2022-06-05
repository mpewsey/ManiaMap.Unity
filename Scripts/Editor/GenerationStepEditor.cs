using UnityEditor;

namespace MPewsey.ManiaMap.Unity.Editor
{
    [CustomEditor(typeof(GenerationStep), true)]
    public class GenerationStepEditor : UnityEditor.Editor
    {
        private const string LongDash = "\u2012";

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            DrawInputNamesLabel();
            DrawOutputNamesLabel();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInputNamesLabel()
        {
            var step = GetGenerationStep();
            var names = step.InputNames();
            var label = names.Length > 0 ? string.Join(", ", names) : LongDash;
            EditorGUILayout.LabelField("Input Names", label);
        }

        private void DrawOutputNamesLabel()
        {
            var step = GetGenerationStep();
            var names = step.OutputNames();
            var label = names.Length > 0 ? string.Join(", ", names) : LongDash;
            EditorGUILayout.LabelField("Output Names", label);
        }

        private GenerationStep GetGenerationStep()
        {
            return (GenerationStep)serializedObject.targetObject;
        }
    }
}
