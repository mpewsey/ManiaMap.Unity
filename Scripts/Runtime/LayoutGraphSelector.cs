using MPewsey.ManiaMap.Generators;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A generation step for drawing a random layout graph in a pipeline.
    /// </summary>
    public class LayoutGraphSelector : GenerationStep
    {
        /// <inheritdoc/>
        public override IGenerationStep GetStep()
        {
            return new Generators.LayoutGraphSelector();
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
