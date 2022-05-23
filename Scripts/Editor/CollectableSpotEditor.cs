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
                AssignClosestCell();
                Debug.Log($"<color=#00FF00><b>Assigned to cell: {GetCollectableSpot().Cell.name}.</b></color>");
            }
        }

        private void AssignClosestCell()
        {
            GetCollectableSpot().AssignClosestCell();
        }
    }
}
