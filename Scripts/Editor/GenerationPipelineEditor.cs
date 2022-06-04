using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    public static class GenerationPipelineEditor
    {
        [MenuItem("GameObject/Mania Map/Generation Pipeline", priority = 20)]
        public static void CreatePipeline()
        {
            var obj = new GameObject("Generation Pipeline");
            obj.AddComponent<GenerationPipeline>();
            var layoutId = obj.AddComponent<GenerationIntInput>();
            layoutId.Name = "LayoutId";
            obj.transform.SetParent(Selection.activeTransform);

            var steps = new GameObject("<Steps>");
            steps.AddComponent<LayoutGraphSelector>();
            steps.AddComponent<LayoutGraphRandomizer>();
            steps.AddComponent<LayoutGenerator>();
            steps.AddComponent<CollectableGenerator>();
            steps.transform.SetParent(obj.transform);
        }
    }
}
