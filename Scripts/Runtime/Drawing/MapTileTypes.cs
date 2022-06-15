using System;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    /// <summary>
    /// A type corresponding to a map tile.
    /// </summary>
    [Flags]
    public enum MapTileTypes
    {
        None,
        Grid = 1 << 0,
        NorthDoor = 1 << 1,
        SouthDoor = 1 << 2,
        EastDoor = 1 << 3,
        WestDoor = 1 << 4,
        TopDoor = 1 << 5,
        BottomDoor = 1 << 6,
        NorthWall = 1 << 7,
        SouthWall = 1 << 8,
        EastWall = 1 << 9,
        WestWall = 1 << 10,
    }
}
