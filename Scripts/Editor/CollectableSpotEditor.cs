using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CollectableSpot))]
    public class CollectableSpotEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawAssignClosestCellButton();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }

        private CollectableSpot GetCollectableSpot()
        {
            return (CollectableSpot)serializedObject.targetObject;
        }

        private void DrawAssignClosestCellButton()
        {
            if (GUILayout.Button("Assign Closest Cell"))
            {
                var spot = GetCollectableSpot();
                var cell = spot.FindClosestCell();
                spot.Cell = cell;
                var name = cell == null ? "None" : cell.name;
                Debug.Log($"<color=#00FF00><b>Assigned to cell: {name}.</b></color>");
            }
        }

        [MenuItem("GameObject/Mania Map/Collectable Spot")]
        public static void CreateCollectableSpot()
        {
            var obj = new GameObject("Collectable Spot");
            obj.AddComponent<CollectableSpot>();
            obj.transform.SetParent(Selection.activeTransform);
        }
    }
}
