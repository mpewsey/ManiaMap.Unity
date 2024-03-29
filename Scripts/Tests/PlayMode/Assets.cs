using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public static class Assets
    {
        // Package Path
        public const string PackagePath = "ManiaMap/";

        // Maps
        public const string LayoutMapPath = PackagePath + "LayoutMap";
        public const string LayoutTilemapPath = PackagePath + "LayoutTilemap";

        // Generation Pipelines
        public const string BigLayoutPath = PackagePath + "BigLayoutGenerator";
        public const string CrossLayoutPath = PackagePath + "CrossLayoutGenerator";
        public const string GeekLayoutPath = PackagePath + "GeekLayoutGenerator";
        public const string LoopLayoutPath = PackagePath + "LoopLayoutGenerator";
        public const string StackedLoopLayoutPath = PackagePath + "StackedLoopLayoutGenerator";

        // Rooms
        public const string Angle3x4RoomPath = PackagePath + "Rooms/Angle3x4Room";

        /// <summary>
        /// Destroys all active GameObjects.
        /// </summary>
        public static void DestroyAllGameObjects()
        {
            foreach (var obj in Object.FindObjectsOfType<GameObject>(true))
            {
                if (obj != null && obj.transform.parent == null)
                    Object.DestroyImmediate(obj);
            }
        }

        /// <summary>
        /// Loads the prefab from the specified resources path.
        /// </summary>
        /// <param name="path">The resources path.</param>
        public static GameObject LoadPrefab(string path)
        {
            return Resources.Load<GameObject>(path);
        }

        /// <summary>
        /// Instantiates the prefab from the specified resources path.
        /// </summary>
        /// <param name="path">The resources path.</param>
        public static T InstantiatePrefab<T>(string path) where T : MonoBehaviour
        {
            var prefab = LoadPrefab(path);
            var obj = Object.Instantiate(prefab);
            return obj.GetComponent<T>();
        }
    }
}