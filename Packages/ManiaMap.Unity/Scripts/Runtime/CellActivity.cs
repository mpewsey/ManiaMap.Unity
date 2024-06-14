namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// The cell activity.
    /// </summary>
    public enum CellActivity
    {
        None, /// No cell activity change.
        Activate, /// Activates a cell.
        Deactivate, /// Deactivates a cell.
        Toggle, /// Toggles the activity of a cell between activated and deactivated and vice versa.
    }
}