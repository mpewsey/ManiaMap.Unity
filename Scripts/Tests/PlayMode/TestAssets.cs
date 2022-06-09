using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public static class TestAssets
    {
        private const string PrefabsDirectory = "Packages/com.mpewsey.maniamap.unity/Prefabs";

        public static GenerationPipeline LoadGenerationPipeline(string path, Transform parent)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Assert.IsNotNull(prefab);
            var obj = Object.Instantiate(prefab, parent);
            var generator = obj.GetComponent<GenerationPipeline>();
            Assert.IsNotNull(generator);
            return generator;
        }

        public static GenerationPipeline LoadBigLayoutGenerator(Transform parent)
        {
            var path = Path.Combine(PrefabsDirectory, "BigLayout/BigLayoutGenerator.prefab");
            return LoadGenerationPipeline(path, parent);
        }

        public static GenerationPipeline LoadCrossLayoutGenerator(Transform parent)
        {
            var path = Path.Combine(PrefabsDirectory, "CrossLayout/CrossLayoutGenerator.prefab");
            return LoadGenerationPipeline(path, parent);
        }

        public static GenerationPipeline LoadGeekLayoutGenerator(Transform parent)
        {
            var path = Path.Combine(PrefabsDirectory, "GeekLayout/GeekLayoutGenerator.prefab");
            return LoadGenerationPipeline(path, parent);
        }

        public static GenerationPipeline LoadLoopLayoutGenerator(Transform parent)
        {
            var path = Path.Combine(PrefabsDirectory, "LoopLayout/LoopLayoutGenerator.prefab");
            return LoadGenerationPipeline(path, parent);
        }

        public static GenerationPipeline LoadStackedLoopLayoutGenerator(Transform parent)
        {
            var path = Path.Combine(PrefabsDirectory, "StackedLoopLayout/StackedLoopLayoutGenerator.prefab");
            return LoadGenerationPipeline(path, parent);
        }
    }
}