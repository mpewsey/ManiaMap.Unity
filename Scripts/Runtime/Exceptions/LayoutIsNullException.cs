using System;

namespace MPewsey.ManiaMap.Unity.Exceptions
{
    /// <summary>
    /// Raised if the layout is null.
    /// </summary>
    public class LayoutIsNullException : Exception
    {
        public LayoutIsNullException(string message) : base(message)
        {

        }
    }
}
