using MPewsey.ManiaMap.Unity.Drawing;
using NUnit.Framework;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public static class TestAssets
    {
        private const string PrefabsDirectory = "Packages/com.mpewsey.maniamap.unity/Prefabs";

        public static T InstantiatePrefab<T>(string path, Transform parent)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Assert.IsNotNull(prefab);
            var obj = Object.Instantiate(prefab, parent);
            var component = obj.GetComponent<T>();
            Assert.IsNotNull(component);
            return component;
        }

        public static LayoutMap LoadLayoutMap(Transform parent)
        {
            var path = Path.Combine(PrefabsDirectory, "LayoutMap.prefab");
            return InstantiatePrefab<LayoutMap>(path, parent);
        }

        public static Room LoadAngle3x4Room(Transform parent)
        {
            var path = Path.Combine(PrefabsDirectory, "Angle3x4Room.prefab");
            return InstantiatePrefab<Room>(path, parent);
        }

        public static GenerationPipeline LoadBigLayoutGenerator(Transform parent)
        {
            var path = Path.Combine(PrefabsDirectory, "BigLayout/BigLayoutGenerator.prefab");
            return InstantiatePrefab<GenerationPipeline>(path, parent);
        }

        public static GenerationPipeline LoadCrossLayoutGenerator(Transform parent)
        {
            var path = Path.Combine(PrefabsDirectory, "CrossLayout/CrossLayoutGenerator.prefab");
            return InstantiatePrefab<GenerationPipeline>(path, parent);
        }

        public static GenerationPipeline LoadGeekLayoutGenerator(Transform parent)
        {
            var path = Path.Combine(PrefabsDirectory, "GeekLayout/GeekLayoutGenerator.prefab");
            return InstantiatePrefab<GenerationPipeline>(path, parent);
        }

        public static GenerationPipeline LoadLoopLayoutGenerator(Transform parent)
        {
            var path = Path.Combine(PrefabsDirectory, "LoopLayout/LoopLayoutGenerator.prefab");
            return InstantiatePrefab<GenerationPipeline>(path, parent);
        }

        public static GenerationPipeline LoadStackedLoopLayoutGenerator(Transform parent)
        {
            var path = Path.Combine(PrefabsDirectory, "StackedLoopLayout/StackedLoopLayoutGenerator.prefab");
            return InstantiatePrefab<GenerationPipeline>(path, parent);
        }
    }
}