using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Editor
{
    /// <summary>
    /// The BatchUpdaterTool editor.
    /// </summary>
    [CustomEditor(typeof(BatchUpdaterTool))]
    public class BatchUpdaterToolEditor : UnityEditor.Editor
    {
        [MenuItem("Mania Map/Create Batch Updater Tool Settings", priority = 0)]
        public static void CreateBatchUpdaterToolSettings()
        {
            var settings = Resources.Load<BatchUpdaterTool>("ManiaMap/BatchUpdaterTool");

            if (settings != null)
            {
                Debug.Log("Settings file already exists.");
                EditorGUIUtility.PingObject(settings);
                return;
            }

            FileUtility.CreateDirectory("Assets/Resources/ManiaMap");
            settings = CreateInstance<BatchUpdaterTool>();
            var path = "Assets/Resources/ManiaMap/BatchUpdaterTool.asset";
            AssetDatabase.CreateAsset(settings, path);
            Debug.Log($"<color=#00FF00><b>Batch Updater Tool settings created at: {path}</b></color>");
            EditorGUIUtility.PingObject(settings);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (GUILayout.Button("Batch Update Room Templates"))
            {
                var tool = (BatchUpdaterTool)serializedObject.targetObject;
                tool.BatchUpdateRoomTemplates();
            }

            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }
    }
}