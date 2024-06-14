using System;

namespace MPewsey.ManiaMapUnity.Exceptions
{
    /// <summary>
    /// Raised when a room template has not been initialized.
    /// </summary>
    public class RoomTemplateNotInitializedException : Exception
    {
        /// <inheritdoc/>
        public RoomTemplateNotInitializedException(string message) : base(message)
        {

        }
    }
}