using System;

namespace MPewsey.ManiaMap.Unity.Exceptions
{
    /// <summary>
    /// Raised if the layout state is null.
    /// </summary>
    public class LayoutStateIsNullException : Exception
    {
        public LayoutStateIsNullException(string message) : base(message)
        {

        }
    }
}
