using MPewsey.Common.Pipelines;

namespace MPewsey.ManiaMap.Unity.Generators
{
    /// <summary>
    /// A generation step for randomizing a layout graph based on the specified parameters.
    /// </summary>
    public class LayoutGraphRandomizer : GenerationStep
    {
        /// <inheritdoc/>
        public override IPipelineStep GetStep()
        {
            return new ManiaMap.Generators.LayoutGraphRandomizer();
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