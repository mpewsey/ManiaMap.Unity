using System;

namespace MPewsey.ManiaMap.Unity.Exceptions
{
    /// <summary>
    /// Raised when a room has not been initialized.
    /// </summary>
    public class RoomNotInitializedException : Exception
    {
        public RoomNotInitializedException(string message) : base(message)
        {

        }
    }
}