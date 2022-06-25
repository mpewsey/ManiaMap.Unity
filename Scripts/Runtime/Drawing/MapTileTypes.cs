using System;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    /// <summary>
    /// A type corresponding to various map tiles.
    /// </summary>
    [Flags]
    public enum MapTileTypes
    {
        /// No tile.
        None,
        /// The grid tile.
        Grid = 1 << 0,
        /// The north door tile.
        NorthDoor = 1 << 1,
        /// The south door tile.
        SouthDoor = 1 << 2,
        /// The east door tile.
        EastDoor = 1 << 3,
        /// The west door tile.
        WestDoor = 1 << 4,
        /// The top door tile.
        TopDoor = 1 << 5,
        /// The bottom door tile.
        BottomDoor = 1 << 6,
        /// The north wall tile.
        NorthWall = 1 << 7,
        /// The south wall tile.
        SouthWall = 1 << 8,
        /// The east wall tile.
        EastWall = 1 << 9,
        /// The west wall tile.
        WestWall = 1 << 10,
    }
}
