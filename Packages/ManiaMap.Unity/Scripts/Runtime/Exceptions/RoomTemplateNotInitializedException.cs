using System;

namespace MPewsey.ManiaMapUnity.Exceptions
{
    /// <summary>
    /// Raised when a room template has not been initialized.
    /// </summary>
    public class RoomTemplateNotInitializedException : Exception
    {
        public RoomTemplateNotInitializedException(string message) : base(message)
        {

        }
    }
}