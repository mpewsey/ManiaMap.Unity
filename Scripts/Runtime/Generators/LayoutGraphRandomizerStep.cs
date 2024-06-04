using MPewsey.Common.Pipelines;
using MPewsey.ManiaMap.Generators;

namespace MPewsey.ManiaMapUnity.Generators
{
    /// <summary>
    /// A generation step for randomizing a layout graph based on the specified parameters.
    /// </summary>
    public class LayoutGraphRandomizerStep : GenerationStep
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