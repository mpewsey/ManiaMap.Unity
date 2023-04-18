using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Generators.Editor
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

            var inputs = new GameObject("<Inputs>");
            inputs.transform.SetParent(obj.transform);
            pipeline.InputsContainer = inputs;

            var layoutId = inputs.AddComponent<GenerationIntInput>();
            layoutId.Name = "LayoutId";
            layoutId.Value = Random.Range(1, int.MaxValue);

            inputs.AddComponent<RandomSeedInput>();
            inputs.AddComponent<LayoutGraphsInput>();
            inputs.AddComponent<CollectableGroupsInput>();

            var steps = new GameObject("<Steps>");
            steps.transform.SetParent(obj.transform);
            pipeline.StepsContainer = steps;
            steps.AddComponent<LayoutGraphSelectorBehavior>();
            steps.AddComponent<LayoutGraphRandomizerBehavior>();
            steps.AddComponent<LayoutGeneratorBehavior>();
            steps.AddComponent<CollectableGeneratorBehavior>();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            DrawPipelineErrorBox();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Returns the target generation pipeline.
        /// </summary>
        private GenerationPipeline GetGenerationPipeline()
        {
            return (GenerationPipeline)serializedObject.targetObject;
        }

        /// <summary>
        /// Draws error boxes for the generation inputs and generation steps if there are
        /// errors in their input or output names.
        /// </summary>
        private void DrawPipelineErrorBox()
        {
            var names = new HashSet<string>(GetGenerationPipeline().ManualInputNames);
            DrawGenerationInputErrorBox(names);
            DrawGenerationStepErrorBox(names);
        }

        /// <summary>
        /// Draws the generation pipeline error box.
        /// </summary>
        /// <param name="names">A set of argument names.</param>
        private void DrawGenerationInputErrorBox(HashSet<string> names)
        {
            var messages = new List<string>() { "Inputs contain errors:" };
            var pipeline = GetGenerationPipeline();
            var inputs = pipeline.GetInputs();

            foreach (var input in inputs)
            {
                foreach (var name in input.OutputNames())
                {
                    if (!names.Add(name))
                        messages.Add($"  * Duplicate input name: {name} <{input.GetType().Name}>.");
                }
            }

            if (messages.Count <= 1)
                return;

            EditorGUILayout.HelpBox(string.Join('\n', messages), MessageType.Error, true);
        }

        /// <summary>
        /// Draws the generation step error box.
        /// </summary>
        /// <param name="names">A set of argument names.</param>
        private void DrawGenerationStepErrorBox(HashSet<string> names)
        {
            var messages = new List<string>() { "Steps contain errors:" };
            var pipeline = GetGenerationPipeline();
            var steps = pipeline.GetSteps();

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

            if (messages.Count <= 1)
                return;

            EditorGUILayout.HelpBox(string.Join('\n', messages), MessageType.Error, true);
        }
    }
}
