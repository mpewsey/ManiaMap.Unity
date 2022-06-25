namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A generation step for randomizing a layout graph based on the specified parameters.
    /// </summary>
    public class LayoutGraphRandomizer : GenerationStep
    {
        /// <inheritdoc/>
        public override IGenerationStep GetStep()
        {
            return new ManiaMap.LayoutGraphRandomizer();
        }

        /// <inheritdoc/>
        public override string[] InputNames()
        {
            return new string[] { "LayoutGraph", "RandomSeed" };
        }

        /// <inheritdoc/>
        public override string[] OutputNames()
        {
            return new string[] { "LayoutGraph" };
        }
    }
}