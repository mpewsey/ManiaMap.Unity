using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The Door editor.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DoorBehavior))]
    public class DoorBehaviorEditor : CellChildEditor
    {
        /// <summary>
        /// Creates a new door Game Object.
        /// </summary>
        [MenuItem("GameObject/Mania Map/Door", priority = 20)]
        [MenuItem("Mania Map/Create Door", priority = 100)]
        public static void CreateDoor()
        {
            var obj = new GameObject("Door");
            obj.transform.SetParent(Selection.activeTransform);
            obj.AddComponent<DoorBehavior>();
        }
    }
}