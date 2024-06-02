using MPewsey.Common.Pipelines;
using MPewsey.ManiaMap.Generators;

namespace MPewsey.ManiaMapUnity.Generators
{
    /// <summary>
    /// A generation step for drawing a random layout graph in a pipeline.
    /// </summary>
    public class LayoutGraphSelectorBehavior : GenerationStep
    {
        /// <inheritdoc/>
        public override IPipelineStep GetStep()
        {
            return new LayoutGraphSelector();
        }

        /// <inheritdoc/>
        public override string[] InputNames()
        {
            return new string[] { "LayoutGraphs", "RandomSeed" };
        }

        /// <inheritdoc/>
        public override string[] OutputNames()
        {
            return new string[] { "LayoutGraph" };
        }
    }
}
