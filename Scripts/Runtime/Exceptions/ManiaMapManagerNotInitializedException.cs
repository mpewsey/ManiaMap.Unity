namespace MPewsey.ManiaMap.Unity.Exceptions
{
    /// <summary>
    /// Raised when data is requested from the Mania Map Manager but it has not been initialized.
    /// </summary>
    public class ManiaMapManagerNotInitializedException : System.Exception
    {
        public ManiaMapManagerNotInitializedException(string message) : base(message)
        {

        }
    }
}