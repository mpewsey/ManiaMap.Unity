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
        public static string GetPackagePath(string path)
        {
            return Path.Combine("Packages/com.mpewsey.maniamap.unity", path);
        }

        public static void LoadEmptyScene()
        {
            var path = GetPackagePath("Scenes/EmptyScene.unity");
            EditorSceneManager.LoadSceneInPlayMode(path, new LoadSceneParameters());
        }

        public static T InstantiatePrefab<T>(string path)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var obj = Object.Instantiate(prefab);
            return obj.GetComponent<T>();
        }

        public static LayoutMap InstantiateLayoutMap()
        {
            var path = GetPackagePath("Prefabs/LayoutMap.prefab");
            return InstantiatePrefab<LayoutMap>(path);
        }

        public static Room InstantiateAngle3x4Room()
        {
            var path = GetPackagePath("Prefabs/Rooms/Angle3x4Room.prefab");
            return InstantiatePrefab<Room>(path);
        }

        public static GenerationPipeline InstantiateBigLayoutGenerator()
        {
            var path = GetPackagePath("Prefabs/BigLayout/BigLayoutGenerator.prefab");
            return InstantiatePrefab<GenerationPipeline>(path);
        }

        public static GenerationPipeline InstantiateCrossLayoutGenerator()
        {
            var path = GetPackagePath("Prefabs/CrossLayout/CrossLayoutGenerator.prefab");
            return InstantiatePrefab<GenerationPipeline>(path);
        }

        public static GenerationPipeline InstantiateGeekLayoutGenerator()
        {
            var path = GetPackagePath("Prefabs/GeekLayout/GeekLayoutGenerator.prefab");
            return InstantiatePrefab<GenerationPipeline>(path);
        }

        public static GenerationPipeline InstantiateLoopLayoutGenerator()
        {
            var path = GetPackagePath("Prefabs/LoopLayout/LoopLayoutGenerator.prefab");
            return InstantiatePrefab<GenerationPipeline>(path);
        }

        public static GenerationPipeline InstantiateStackedLoopLayoutGenerator()
        {
            var path = GetPackagePath("Prefabs/StackedLoopLayout/StackedLoopLayoutGenerator.prefab");
            return InstantiatePrefab<GenerationPipeline>(path);
        }
    }
}