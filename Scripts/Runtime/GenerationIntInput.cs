namespace MPewsey.ManiaMap.Unity
{
    public class GenerationIntInput : GenerationNamedInput<int>
    {
        public override string[] OutputNames()
        {
            return new string[] { Name };
        }
    }
}
