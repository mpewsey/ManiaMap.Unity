using System;

namespace MPewsey.ManiaMapUnity.Exceptions
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
