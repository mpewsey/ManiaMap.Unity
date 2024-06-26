using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Generators.Editor
{
    /// <summary>
    /// The GenerationPipeline editor.
    /// </summary>
    [CustomEditor(typeof(GenerationPipeline))]
    public class GenerationPipelineEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Creates a new generation pipeline Game Object.
        /// </summary>
        [MenuItem("GameObject/Mania Map/Generation Pipeline", priority = 20)]
        [MenuItem("Mania Map/Create Generation Pipeline", priority = 100)]
        public static void CreatePipeline()
        {
            var obj = new GameObject("Generation Pipeline");
            obj.transform.SetParent(Selection.activeTransform);
            var pipeline = obj.AddComponent<GenerationPipeline>();

            var inputs = new GameObject("Inputs");
            inputs.transform.SetParent(obj.transform);

            var layoutId = inputs.AddComponent<GenerationIntInput>();
            layoutId.Name = "LayoutId";
            layoutId.Value = Random.Range(1, int.MaxValue);

            inputs.AddComponent<RandomSeedInput>();
            inputs.AddComponent<LayoutGraphsInput>();
            inputs.AddComponent<CollectableGroupsInput>();

            var steps = new GameObject("Steps");
            steps.transform.SetParent(obj.transform);
            steps.AddComponent<LayoutGraphSelectorStep>();
            steps.AddComponent<LayoutGraphRandomizerStep>();
            steps.AddComponent<LayoutGeneratorStep>();
            steps.AddComponent<CollectableGeneratorStep>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            DrawPipelineErrorBox();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws error boxes for the generation inputs and generation steps if there are
        /// errors in their input or output names.
        /// </summary>
        private void DrawPipelineErrorBox()
        {
            var pipeline = (GenerationPipeline)serializedObject.targetObject;
            var names = new HashSet<string>(pipeline.ManualInputNames);
            DrawGenerationInputErrorBox(pipeline, names);
            DrawGenerationStepErrorBox(pipeline, names);
        }

        /// <summary>
        /// Draws the generation pipeline error box.
        /// </summary>
        /// <param name="names">A set of argument names.</param>
        /// <param name="pipeline">The generation pipeline.</param>
        private static void DrawGenerationInputErrorBox(GenerationPipeline pipeline, HashSet<string> names)
        {
            var messages = new List<string>() { "Inputs contain errors:" };
            var inputs = pipeline.FindInputComponents();

            foreach (var input in inputs)
            {
                foreach (var name in input.OutputNames())
                {
                    if (!names.Add(name))
                        messages.Add($"  * Duplicate input name: {name} <{input.GetType().Name}>.");
                }
            }

            if (messages.Count > 1)
                EditorGUILayout.HelpBox(string.Join('\n', messages), MessageType.Error, true);
        }

        /// <summary>
        /// Draws the generation step error box.
        /// </summary>
        /// <param name="names">A set of argument names.</param>
        /// <param name="pipeline">The generation pipeline.</param>
        private static void DrawGenerationStepErrorBox(GenerationPipeline pipeline, HashSet<string> names)
        {
            var messages = new List<string>() { "Steps contain errors:" };
            var steps = pipeline.FindStepComponents();

            foreach (var step in steps)
            {
                foreach (var name in step.InputNames())
                {
                    if (!names.Contains(name))
                        messages.Add($"  * Missing input name: {name} <{step.GetType().Name}>.");
                }

                foreach (var name in step.OutputNames())
                {
                    names.Add(name);
                }
            }

            if (messages.Count > 1)
                EditorGUILayout.HelpBox(string.Join('\n', messages), MessageType.Error, true);
        }
    }
}
