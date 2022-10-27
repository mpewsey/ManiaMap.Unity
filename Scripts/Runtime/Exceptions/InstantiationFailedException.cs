using System;

namespace MPewsey.ManiaMap.Unity.Exceptions
{
    /// <summary>
    /// Raised when the instantiation of a GameObject fails.
    /// </summary>
    public class InstantiationFailedException : Exception
    {
        public InstantiationFailedException(string message) : base(message)
        {

        }
    }
}