using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    public class LayoutGraphWindowSelectTool : ILayoutGraphWindowTool
    {
        private const int LeftMouseButton = 0;
        private const int RightMouseButton = 1;

        private bool Dragging { get; set; }
        private Vector2 DragStart { get; set; }
        private Object DownObject { get; set; }

        public void OnAreaEvent()
        {
            switch (Event.current.type)
            {
                case EventType.MouseUp when Event.current.button == LeftMouseButton:
                    OnAreaLeftMouseUp();
                    break;
                case EventType.MouseUp when Event.current.button == RightMouseButton:
                    OnAreaRightMouseUp();
                    break;
                case EventType.MouseDown when Event.current.button == LeftMouseButton:
                    OnAreaLeftMouseDown();
                    break;
            }
        }

        private void OnAreaRightMouseUp()
        {
            GUI.FocusControl(null);
            Dragging = false;
            DownObject = null;
            var window = EditorWindow.GetWindow<NewLayoutGraphWindow>();
            window.SelectedEdges.Clear();
            window.SelectedNodes.Clear();
            Event.current.Use();
        }

        private void OnAreaLeftMouseUp()
        {
            Dragging = false;
            DownObject = null;
        }

        private void OnAreaLeftMouseDown()
        {
            Dragging = true;
            DownObject = null;
            DragStart = Event.current.mousePosition;
        }

        public void OnEdgeEvent(LayoutEdge edge)
        {
            switch (Event.current.type)
            {
                case EventType.MouseUp when Event.current.button == LeftMouseButton:
                    OnElementLeftMouseUp(edge);
                    break;
                case EventType.MouseUp when Event.current.button == RightMouseButton:
                    OnElementRightMouseUp();
                    break;
                case EventType.MouseDown when Event.current.button == LeftMouseButton:
                    OnElementLeftMouseDown(edge);
                    break;
            }
        }

        public void OnGUIEnd()
        {
            RepaintIfDragging();
        }

        private void RepaintIfDragging()
        {
            if (Dragging)
            {
                var window = EditorWindow.GetWindow<NewLayoutGraphWindow>();
                window.Repaint();
            }
        }

        public void OnDrawPlotEnd()
        {
            DrawDragArea();
        }

        private void DrawDragArea()
        {
            if (Dragging)
            {
                var window = EditorWindow.GetWindow<NewLayoutGraphWindow>();
                window.DrawDragArea(DragStart);
            }
        }

        public void OnLostFocus()
        {
            Dragging = false;
            DownObject = null;
        }

        public void OnNodeEvent(LayoutNode node)
        {
            switch (Event.current.type)
            {
                case EventType.MouseUp when Event.current.button == LeftMouseButton:
                    OnElementLeftMouseUp(node);
                    break;
                case EventType.MouseUp when Event.current.button == RightMouseButton:
                    OnElementRightMouseUp();
                    break;
                case EventType.MouseDown when Event.current.button == LeftMouseButton:
                    OnElementLeftMouseDown(node);
                    break;
            }
        }

        private void OnElementRightMouseUp()
        {
            OnAreaRightMouseUp();
        }

        private void OnElementLeftMouseUp(Object element)
        {
            if (element == DownObject && element != null)
            {
                var window = EditorWindow.GetWindow<NewLayoutGraphWindow>();
                window.ToggleElementSelection(element);
            }

            GUI.FocusControl(null);
            Dragging = false;
            DownObject = null;
            Event.current.Use();
        }

        private void OnElementLeftMouseDown(Object element)
        {
            GUI.FocusControl(null);
            Dragging = false;
            DownObject = element;
            Event.current.Use();
        }
    }
}
