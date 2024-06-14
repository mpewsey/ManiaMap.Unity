using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The DoorComponent editor.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DoorComponent))]
    public class DoorComponentEditor : CellChildEditor
    {
        [MenuItem("GameObject/Mania Map/Door", priority = 20)]
        [MenuItem("Mania Map/Create Door", priority = 100)]
        public static void CreateDoor()
        {
            var obj = new GameObject("Door");
            obj.transform.SetParent(Selection.activeTransform);
            obj.AddComponent<DoorComponent>();
        }
    }
}