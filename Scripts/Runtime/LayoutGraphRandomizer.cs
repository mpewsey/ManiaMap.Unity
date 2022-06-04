namespace MPewsey.ManiaMap.Unity
{
    public class LayoutGraphRandomizer : GenerationStep
    {
        public override IGenerationStep GetStep()
        {
            return new ManiaMap.LayoutGraphRandomizer();
        }

        public override string[] InputNames()
        {
            return new string[] { "LayoutGraph", "RandomSeed" };
        }

        public override string[] OutputNames()
        {
            return new string[] { "LayoutGraph" };
        }
    }
}