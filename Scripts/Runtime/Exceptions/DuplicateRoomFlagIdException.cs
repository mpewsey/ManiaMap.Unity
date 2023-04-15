namespace MPewsey.ManiaMap.Unity.Exceptions
{
    /// <summary>
    /// Exception raised when a duplicate room flag ID exists.
    /// </summary>
    public class DuplicateRoomFlagIdException : System.Exception
    {
        public DuplicateRoomFlagIdException(string message) : base(message)
        {

        }
    }
}