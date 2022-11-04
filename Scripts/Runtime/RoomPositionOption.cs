namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// The room position option.
    /// </summary>
    public enum RoomPositionOption
    {
        /// Room position is based on the Mania Map Manager settings.
        UseManagerSettings,
        /// Rooms are instantiated at the origin of their parent.
        Origin,
        /// Rooms have their local positions set to their positions in the layout.
        LayoutPosition,
    }
}
