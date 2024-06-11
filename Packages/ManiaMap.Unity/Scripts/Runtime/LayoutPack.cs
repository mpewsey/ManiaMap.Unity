using MPewsey.Common.Mathematics;
using MPewsey.ManiaMap;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// A manager for maintaining the current map data and state.
    /// </summary>
    public class LayoutPack
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
        /// The applied settings.
        /// </summary>
        public ManiaMapSettings Settings { get; }

        /// <summary>
        /// A dictionary of door connections by room ID.
        /// </summary>
        private Dictionary<Uid, List<DoorConnection>> RoomConnections { get; } = new Dictionary<Uid, List<DoorConnection>>();

        /// <summary>
        /// A dictionary of rooms by layer coordinate.
        /// </summary>
        private Dictionary<int, List<Room>> RoomsByLayer { get; } = new Dictionary<int, List<Room>>();

        /// <summary>
        /// A dictionary of door positions by room ID.
        /// </summary>
        private Dictionary<Uid, List<DoorPosition>> RoomDoors { get; } = new Dictionary<Uid, List<DoorPosition>>();

        /// <summary>
        /// The bounds of the layout.
        /// </summary>
        public RectangleInt LayoutBounds { get; }

        public LayoutPack(Layout layout, LayoutState layoutState, ManiaMapSettings settings = null)
        {
            if (layout == null)
                throw new System.ArgumentException("Layout cannot be null.");
            if (layoutState == null)
                throw new System.ArgumentException("Layout state cannot be null.");
            if (layout.Id != layoutState.Id)
                throw new System.ArgumentException($"Layout and layout state ID's do not match: (Layout ID = {layout.Id}, Layout State ID = {layoutState.Id})");

            Layout = layout;
            LayoutState = layoutState;
            Settings = settings != null ? settings : ScriptableObject.CreateInstance<ManiaMapSettings>();
            RoomConnections = layout.GetRoomConnections();
            RoomsByLayer = layout.GetRoomsByLayer();
            RoomDoors = layout.GetRoomDoors();
            LayoutBounds = layout.GetBounds();
        }

        /// <summary>
        /// Returns a list of door connections with a room for the specified room ID.
        /// If the room ID does not exist, returns an empty list.
        /// </summary>
        /// <param name="id">The room ID.</param>
        public IReadOnlyList<DoorConnection> GetDoorConnections(Uid id)
        {
            if (RoomConnections.TryGetValue(id, out var connections))
                return connections;
            return System.Array.Empty<DoorConnection>();
        }

        /// <summary>
        /// Returns a list of rooms corresponding to the specified layer (z) coordinate.
        /// If the layer does not exist, returns an empty list.
        /// </summary>
        /// <param name="z">The layer coordinate.</param>
        public IReadOnlyList<Room> GetRoomsInLayer(int z)
        {
            if (RoomsByLayer.TryGetValue(z, out var rooms))
                return rooms;
            return System.Array.Empty<Room>();
        }

        /// <summary>
        /// Returns a list of door positions for the specified room ID.
        /// If the room ID does not exist, returns an empty list.
        /// </summary>
        /// <param name="id">The room ID.</param>
        public IReadOnlyList<DoorPosition> GetRoomDoors(Uid id)
        {
            if (RoomDoors.TryGetValue(id, out var doors))
                return doors;
            return System.Array.Empty<DoorPosition>();
        }

        /// <summary>
        /// Returns true if the door exists for the specified cell index and direction.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="position">The cell index in the room.</param>
        /// <param name="direction">The door direction.</param>
        public bool DoorExists(Uid id, Vector2DInt position, DoorDirection direction)
        {
            foreach (var door in GetRoomDoors(id))
            {
                if (door.Matches(position, direction))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns an enumerable of unique layer (z) coordinates for the layout.
        /// The coordinates are not in any particular order.
        /// </summary>
        public IEnumerable<int> GetLayerCoordinates()
        {
            return RoomsByLayer.Keys;
        }

        /// <summary>
        /// Returns the door connection with the given room ID, position, and direction if it exists.
        /// Returns null if the connection does not exist.
        /// </summary>
        /// <param name="id">The room ID.</param>
        /// <param name="position">The cell position index.</param>
        /// <param name="direction">The door direction.</param>
        public DoorConnection FindDoorConnection(Uid id, Vector2DInt position, DoorDirection direction)
        {
            foreach (var connection in GetDoorConnections(id))
            {
                if (connection.ContainsDoor(id, position, direction))
                    return connection;
            }

            return null;
        }
    }
}