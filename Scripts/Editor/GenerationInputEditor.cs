using UnityEditor;

namespace MPewsey.ManiaMap.Unity.Editor
{
    [CustomEditor(typeof(GenerationInput), true)]
    public class GenerationInputEditor : UnityEditor.Editor
    {
        private const string LongDash = "\u2012";

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            DrawOutputNamesLabel();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawOutputNamesLabel()
        {
            var input = GetGenerationInput();
            var names = input.OutputNames();
            var label = names.Length > 0 ? string.Join(", ", names) : LongDash;
            EditorGUILayout.LabelField("Output Names", label);
        }

        private GenerationInput GetGenerationInput()
        {
            return (GenerationInput)serializedObject.targetObject;
        }
    }
}
