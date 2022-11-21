namespace MPewsey.ManiaMap.Unity.Generators
{
    /// <summary>
    /// A component for adding a named integer input to a GenerationPipeline.
    /// </summary>
    public class GenerationIntInput : GenerationNamedInput<int>
    {
        /// <inheritdoc/>
        public override string[] OutputNames()
        {
            return new string[] { Name };
        }
    }
}
