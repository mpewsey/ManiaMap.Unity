namespace MPewsey.ManiaMap.Unity.Editor
{
    public interface ILayoutGraphWindowTool
    {
        void OnAreaEvent();
        void OnNodeEvent(LayoutNode node);
        void OnEdgeEvent(LayoutEdge edge);
        void OnLostFocus();
        void OnGUIEnd();
        void OnDrawPlotEnd();
    }
}
