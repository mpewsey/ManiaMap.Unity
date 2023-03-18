using MPewsey.Common.Pipelines;
using MPewsey.ManiaMap.Generators;

namespace MPewsey.ManiaMap.Unity.Generators
{
    /// <summary>
    /// A generation step for randomizing a layout graph based on the specified parameters.
    /// </summary>
    public class LayoutGraphRandomizerBehavior : GenerationStep
    {
        /// <inheritdoc/>
        public override IPipelineStep GetStep()
        {
            return new LayoutGraphRandomizer();
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