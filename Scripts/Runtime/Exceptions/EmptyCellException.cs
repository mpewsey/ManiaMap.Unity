using System;

namespace MPewsey.ManiaMap.Unity.Exceptions
{
    public class EmptyCellException : Exception
    {
        public EmptyCellException(string message) : base(message)
        {

        }
    }
}