using System;

namespace MPewsey.ManiaMap.Unity.Exceptions
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
