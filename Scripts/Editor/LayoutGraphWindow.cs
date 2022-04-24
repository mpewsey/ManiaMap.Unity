using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    public class LayoutGraphWindow : EditorWindow
    {
        private const float MenuBarHeight = 25;
        private static SerializedObject SerializedObject { get; set; }

        private static LayoutGraph GetLayoutGraph()
        {
            return SerializedObject.targetObject as LayoutGraph;
        }

        public static void ShowWindow(LayoutGraph graph)
        {
            SerializedObject = new SerializedObject(graph);
            var window = GetWindow<LayoutGraphWindow>();
            window.titleContent = new GUIContent("Layout Graph Editor");
            window.minSize = new Vector2(600, 200);
            window.maxSize = new Vector2(1920, 720);
        }

        private static bool LayoutGraphExists()
        {
            return SerializedObject != null
                && GetLayoutGraph() != null;
        }

        public void OnGUI()
        {
            if (!LayoutGraphExists())
            {
                Close();
                return;
            }
            
            SerializedObject.Update();
            DrawMenuBar();
            SerializedObject.ApplyModifiedProperties();
        }

        private void DrawMenuBar()
        {
            GUILayout.BeginArea(new Rect(0, 0, position.width, MenuBarHeight), GUI.skin.box);
            GUILayout.EndArea();
        }
    }
}
