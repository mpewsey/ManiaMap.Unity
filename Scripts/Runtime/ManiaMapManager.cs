using MPewsey.ManiaMap;
using System.Collections.Generic;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// A manager for maintaining the current map data and state.
    /// </summary>
    public class ManiaMapManager
    {
        public static ManiaMapManager Current { get; private set; }

        /// <summary>
        /// The current layout.
        /// </summary>
        public Layout Layout { get; private set; }

        /// <summary>
        /// The current layout state.
        /// </summary>
        public LayoutState LayoutState { get; private set; }

        /// <summary>
        /// The manager settings.
        /// </summary>
        public ManiaMapSettings Settings { get; private set; }

        /// <summary>
        /// A dictionary of door connections by room ID.
        /// </summary>
        private Dictionary<Uid, List<DoorConnection>> RoomConnections { get; set; } = new Dictionary<Uid, List<DoorConnection>>();

        public static ManiaMapManager Initialize(Layout layout, LayoutState layoutState, ManiaMapSettings settings = null)
        {
            if (layout == null)
                throw new System.ArgumentException("Layout cannot be null.");
            if (layoutState == null)
                throw new System.ArgumentException("Layout state cannot be null.");
            if (layout.Id != layoutState.Id)
                throw new System.ArgumentException($"Layout and layout state ID's do not match: (Layout ID = {layout.Id}, Layout State ID = {layoutState.Id})");

            var manager = new ManiaMapManager()
            {
                Layout = layout,
                LayoutState = layoutState,
                Settings = settings != null ? settings : new ManiaMapSettings(),
                RoomConnections = layout.GetRoomConnections(),
            };

            manager.SetAsCurrent();
            return manager;
        }

        private void SetAsCurrent()
        {
            Current = this;
        }

        /// <summary>
        /// Returns a list of door connections by room ID.
        /// </summary>
        /// <param name="id">The room ID.</param>
        public IReadOnlyList<DoorConnection> GetDoorConnections(Uid id)
        {
            if (RoomConnections.TryGetValue(id, out var connections))
                return connections;
            return System.Array.Empty<DoorConnection>();
        }
    }
}