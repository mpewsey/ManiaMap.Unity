using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public static class Assets
    {
        // Package Path
        public const string PackagePath = "Packages/com.mpewsey.maniamap.unity/";

        // Scenes
        public const string EmptyScenePath = PackagePath + "Scenes/EmptyScene.unity";

        // Maps
        public const string LayoutMapPath = PackagePath + "Prefabs/LayoutMap.prefab";
        public const string LayoutTilemapPath = PackagePath + "Prefabs/LayoutTilemap.prefab";

        // Generation Pipelines
        public const string BigLayoutPath = PackagePath + "Prefabs/BigLayout/BigLayoutGenerator.prefab";
        public const string CrossLayoutPath = PackagePath + "Prefabs/CrossLayout/CrossLayoutGenerator.prefab";
        public const string GeekLayoutPath = PackagePath + "Prefabs/GeekLayout/GeekLayoutGenerator.prefab";
        public const string LoopLayoutPath = PackagePath + "Prefabs/LoopLayout/LoopLayoutGenerator.prefab";
        public const string StackedLoopLayoutPath = PackagePath + "Prefabs/StackedLoopLayout/StackedLoopLayoutGenerator.prefab";

        // Rooms
        public const string Angle3x4RoomPath = PackagePath + "Prefabs/Rooms/Angle3x4Room.prefab";

        public static void LoadEmptyScene()
        {
            EditorSceneManager.LoadSceneInPlayMode(EmptyScenePath, new LoadSceneParameters());
        }

        public static GameObject LoadPrefab(string path)
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }

        public static T InstantiatePrefab<T>(string path) where T : MonoBehaviour
        {
            var prefab = LoadPrefab(path);
            var obj = Object.Instantiate(prefab);
            return obj.GetComponent<T>();
        }
    }
}