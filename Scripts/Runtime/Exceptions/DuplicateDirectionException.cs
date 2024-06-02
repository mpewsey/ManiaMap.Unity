using System;

namespace MPewsey.ManiaMapUnity.Exceptions
{
    /// <summary>
    /// An exception raised when a duplicate direction exists.
    /// </summary>
    public class DuplicateDirectionException : Exception
    {
        public DuplicateDirectionException(string message) : base(message)
        {

        }
    }
}
