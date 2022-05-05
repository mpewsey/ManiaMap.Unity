using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    public interface ILayoutGraphWindowTool
    {
        void OnAreaMouseDown();
        void OnAreaMouseUp();
        void OnNodeMouseDown(LayoutNode node);
        void OnNodeMouseUp(LayoutNode node);
        void OnEdgeMouseDown(LayoutEdge edge);
        void OnEdgeMouseUp(LayoutEdge edge);
        void OnLostFocus();
    }
}
