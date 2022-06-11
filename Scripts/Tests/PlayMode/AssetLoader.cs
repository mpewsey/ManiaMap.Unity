using MPewsey.ManiaMap.Unity.Drawing;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public static class AssetLoader
    {
        private const string PrefabsDirectory = "Packages/com.mpewsey.maniamap.unity/Prefabs";
        private const string SceneDirectory = "Packages/com.mpewsey.maniamap.unity/Scenes";

        public static void LoadEmptyScene()
        {
            var path = Path.Combine(SceneDirectory, "EmptyScene.unity");
            EditorSceneManager.LoadSceneInPlayMode(path, new LoadSceneParameters());
        }

        public static T InstantiatePrefab<T>(string path)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var obj = Object.Instantiate(prefab);
            return obj.GetComponent<T>();
        }

        public static LayoutMap LoadLayoutMap()
        {
            var path = Path.Combine(PrefabsDirectory, "LayoutMap.prefab");
            return InstantiatePrefab<LayoutMap>(path);
        }

        public static Room LoadAngle3x4Room()
        {
            var path = Path.Combine(PrefabsDirectory, "Angle3x4Room.prefab");
            return InstantiatePrefab<Room>(path);
        }

        public static GenerationPipeline LoadBigLayoutGenerator()
        {
            var path = Path.Combine(PrefabsDirectory, "BigLayout/BigLayoutGenerator.prefab");
            return InstantiatePrefab<GenerationPipeline>(path);
        }

        public static GenerationPipeline LoadCrossLayoutGenerator()
        {
            var path = Path.Combine(PrefabsDirectory, "CrossLayout/CrossLayoutGenerator.prefab");
            return InstantiatePrefab<GenerationPipeline>(path);
        }

        public static GenerationPipeline LoadGeekLayoutGenerator()
        {
            var path = Path.Combine(PrefabsDirectory, "GeekLayout/GeekLayoutGenerator.prefab");
            return InstantiatePrefab<GenerationPipeline>(path);
        }

        public static GenerationPipeline LoadLoopLayoutGenerator()
        {
            var path = Path.Combine(PrefabsDirectory, "LoopLayout/LoopLayoutGenerator.prefab");
            return InstantiatePrefab<GenerationPipeline>(path);
        }

        public static GenerationPipeline LoadStackedLoopLayoutGenerator()
        {
            var path = Path.Combine(PrefabsDirectory, "StackedLoopLayout/StackedLoopLayoutGenerator.prefab");
            return InstantiatePrefab<GenerationPipeline>(path);
        }
    }
}