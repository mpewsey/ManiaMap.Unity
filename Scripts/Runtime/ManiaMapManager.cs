using MPewsey.ManiaMap;
using System.Collections.Generic;
using UnityEngine;

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

        public void Initialize(Layout layout, LayoutState layoutState, ManiaMapSettings settings = null)
        {
            if (layout == null)
                throw new System.ArgumentException("Layout cannot be null.");
            if (layoutState == null)
                throw new System.ArgumentException("Layout state cannot be null.");
            if (layout.Id != layoutState.Id)
                throw new System.ArgumentException($"Layout and layout state ID's do not match: (Layout ID = {layout.Id}, Layout State ID = {layoutState.Id})");

            Layout = layout;
            LayoutState = layoutState;
            Settings = settings != null ? settings : ManiaMapSettings.LoadSettings();
            RoomConnections = layout.GetRoomConnections();
        }

        /// <summary>
        /// If the ID is less than or equal to zero, returns a random positive integer. Otherwise, returns the ID.
        /// </summary>
        /// <param name="id">The original ID.</param>
        public static int AutoAssignId(int id)
        {
            if (id <= 0)
                return Random.Range(1, int.MaxValue);
            return id;
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