using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// The Feature editor.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RoomFlag))]
    public class RoomFlagEditor : CellChildEditor
    {
        /// <summary>
        /// Creates a new room flag Game Object.
        /// </summary>
        [MenuItem("GameObject/Mania Map/Room Flag", priority = 20)]
        [MenuItem("Mania Map/Create Room Flag", priority = 100)]
        public static void CreateRoomFlag()
        {
            var obj = new GameObject("Room Flag");
            obj.transform.SetParent(Selection.activeTransform);
            obj.AddComponent<RoomFlag>();
        }
    }
}
