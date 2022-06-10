using System;

namespace MPewsey.ManiaMap.Unity.Exceptions
{
    public class DuplicateDirectionException : Exception
    {
        public DuplicateDirectionException(string message) : base(message)
        {

        }
    }
}
