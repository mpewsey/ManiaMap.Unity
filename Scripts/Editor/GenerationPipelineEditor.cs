using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    [CustomEditor(typeof(GenerationPipeline))]
    public class GenerationPipelineEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            DrawPipelineErrorBox();
            serializedObject.ApplyModifiedProperties();
        }

        private GenerationPipeline GetGenerationPipeline()
        {
            return (GenerationPipeline)serializedObject.targetObject;
        }

        private void DrawPipelineErrorBox()
        {
            var inputMessages = new List<string>();
            inputMessages.Add("Inputs contain errors:");
            var stepMessages = new List<string>();
            stepMessages.Add("Steps contain errors:");
            var arguments = new HashSet<string>() { "RandomSeed" };
            var pipeline = GetGenerationPipeline();
            var inputs = pipeline.GetGenerationInputs();
            var steps = pipeline.GetGenerationSteps();

            foreach (var input in inputs)
            {
                foreach (var name in input.OutputNames())
                {
                    if (!arguments.Add(name))
                        inputMessages.Add($"  * Duplicate input name: {name} <{input.GetType().Name}>.");
                }
            }

            foreach (var step in steps)
            {
                foreach (var name in step.InputNames())
                {
                    if (!arguments.Contains(name))
                        stepMessages.Add($"  * Missing input name: {name} <{step.GetType().Name}>.");
                }

                foreach (var name in step.OutputNames())
                {
                    arguments.Add(name);
                }
            }

            if (inputMessages.Count > 1)
                EditorGUILayout.HelpBox(string.Join('\n', inputMessages), MessageType.Error, true);
            if (stepMessages.Count > 1)
                EditorGUILayout.HelpBox(string.Join('\n', stepMessages), MessageType.Error, true);
        }

        [MenuItem("GameObject/Mania Map/Generation Pipeline", priority = 20)]
        [MenuItem("Mania Map/Create Generation Pipeline", priority = 100)]
        public static void CreatePipeline()
        {
            var obj = new GameObject("Generation Pipeline");
            var pipeline = obj.AddComponent<GenerationPipeline>();

            var inputs = new GameObject("<Inputs>");
            var layoutId = inputs.AddComponent<GenerationIntInput>();
            layoutId.Name = "LayoutId";
            layoutId.Value = Random.Range(1, int.MaxValue);
            inputs.AddComponent<LayoutGraphsInput>();
            inputs.AddComponent<TemplateGroupsInput>();
            inputs.AddComponent<CollectableGroupsInput>();
            inputs.transform.SetParent(Selection.activeTransform);
            inputs.transform.SetParent(obj.transform);
            pipeline.InputsContainer = inputs;

            var steps = new GameObject("<Steps>");
            steps.AddComponent<LayoutGraphSelector>();
            steps.AddComponent<LayoutGraphRandomizer>();
            steps.AddComponent<LayoutGenerator>();
            steps.AddComponent<CollectableGenerator>();
            steps.transform.SetParent(obj.transform);
            pipeline.StepsContainer = steps;
        }
    }
}
