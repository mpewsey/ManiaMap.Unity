using System;

namespace MPewsey.ManiaMap.Unity.Exceptions
{
    /// <summary>
    /// Raised when a room component is missing from a GameObject.
    /// </summary>
    public class MissingRoomComponentException : Exception
    {
        public MissingRoomComponentException(string message) : base(message)
        {

        }
    }
}
