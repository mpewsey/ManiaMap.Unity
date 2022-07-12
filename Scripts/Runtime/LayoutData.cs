using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A containing for storing a layout and its state.
    /// </summary>
    public class LayoutData
    {
        /// <summary>
        /// The layout.
        /// </summary>
        public Layout Layout { get; }

        /// <summary>
        /// The layout state.
        /// </summary>
        public LayoutState LayoutState { get; }

        /// <summary>
        /// The maximum depth used for room clusters.
        /// </summary>
        public int MaxClusterDepth { get; }

        /// <summary>
        /// A dictionary of adjacent rooms by room ID.
        /// </summary>
        private Dictionary<Uid, List<Uid>> RoomAdjacencies { get; }

        /// <summary>
        /// A dictionary of room clusters by room ID.
        /// </summary>
        private Dictionary<Uid, HashSet<Uid>> RoomClusters { get; }

        public LayoutData(Layout layout, LayoutState state, int maxClusterDepth = 1)
        {
            Layout = layout;
            LayoutState = state;
            MaxClusterDepth = Mathf.Max(maxClusterDepth, 1);
            RoomAdjacencies = layout.RoomAdjacencies();
            RoomClusters = layout.FindClusters(MaxClusterDepth);
        }

        /// <summary>
        /// Returns the room in the layout corresponding to the specified ID.
        /// If the ID does not exist, returns null.
        /// </summary>
        /// <param name="id">The room ID.</param>
        public ManiaMap.Room GetRoom(Uid id)
        {
            Layout.Rooms.TryGetValue(id, out ManiaMap.Room room);
            return room;
        }

        /// <summary>
        /// Returns the room state in the layout corresponding to the specified ID.
        /// If the ID does not exist, returns null.
        /// </summary>
        /// <param name="id">The room ID.</param>
        public RoomState GetRoomState(Uid id)
        {
            LayoutState.RoomStates.TryGetValue(id, out RoomState state);
            return state;
        }

        /// <summary>
        /// Returns a list of adjacent room ID's.
        /// </summary>
        /// <param name="id">The room ID for which adjacent rooms will be returned.</param>
        public IReadOnlyList<Uid> GetAdjacentRooms(Uid id)
        {
            if (RoomAdjacencies.TryGetValue(id, out List<Uid> rooms))
                return rooms;
            return System.Array.Empty<Uid>();
        }

        /// <summary>
        /// Returns an enumerable of rooms belonging to the room cluster.
        /// </summary>
        /// <param name="id">The room ID for which the cluster will be returned.</param>
        public IEnumerable<Uid> GetRoomCluster(Uid id)
        {
            if (RoomClusters.TryGetValue(id, out HashSet<Uid> cluster))
                return cluster;
            return System.Array.Empty<Uid>();
        }
    }
}
