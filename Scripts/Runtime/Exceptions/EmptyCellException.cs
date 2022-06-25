using System;

namespace MPewsey.ManiaMap.Unity.Exceptions
{
    /// <summary>
    /// Exception raised when a cell is empty.
    /// </summary>
    public class EmptyCellException : Exception
    {
        public EmptyCellException(string message) : base(message)
        {

        }
    }
}