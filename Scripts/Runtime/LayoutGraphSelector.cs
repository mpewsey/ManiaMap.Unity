namespace MPewsey.ManiaMap.Unity
{
    public class LayoutGraphSelector : GenerationStep
    {
        public override IGenerationStep GetStep()
        {
            return new ManiaMap.LayoutGraphSelector();
        }

        public override string[] InputNames()
        {
            return new string[] { "LayoutGraphs", "RandomSeed" };
        }

        public override string[] OutputNames()
        {
            return new string[] { "LayoutGraph" };
        }
    }
}
