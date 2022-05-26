using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    [CustomEditor(typeof(Door))]
    public class DoorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawAssignClosestCellButton();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }

        private Door GetDoor()
        {
            return (Door)serializedObject.targetObject;
        }

        private void DrawAssignClosestCellButton()
        {
            if (GUILayout.Button("Assign Closest Cell"))
            {
                AssignClosestCell();
            }
        }

        private void AssignClosestCell()
        {
            var door = GetDoor();
            door.Cell = Cell.FindClosestCell(door.transform);
            var name = door.Cell == null ? "None" : door.Cell.name;
            Debug.Log($"<color=#00FF00><b>Assigned to cell: {name}.</b></color>");
        }

        [MenuItem("GameObject/Mania Map/Door")]
        public static void CreateDoor()
        {
            var obj = new GameObject("Door");
            obj.AddComponent<Door>();
            obj.transform.SetParent(Selection.activeTransform);
        }
    }
}