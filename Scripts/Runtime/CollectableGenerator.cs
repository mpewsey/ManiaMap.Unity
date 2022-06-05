using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class CollectableGenerator : GenerationStep
    {
        [SerializeField]
        private float _doorPower = 2;
        public float DoorPower { get => _doorPower; set => _doorPower = value; }

        [SerializeField]
        private float _neighborPower = 1;
        public float NeighborPower { get => _neighborPower; set => _neighborPower = value; }

        [SerializeField]
        private int _initialNeighborWeight = 1000;
        public int InitialNeighborWeight { get => _initialNeighborWeight; set => _initialNeighborWeight = value; }

        public override IGenerationStep GetStep()
        {
            return new ManiaMap.CollectableGenerator(DoorPower, NeighborPower, InitialNeighborWeight);
        }

        public override string[] InputNames()
        {
            return new string[] { "Layout", "CollectableGroups", "RandomSeed" };
        }

        public override string[] OutputNames()
        {
            return System.Array.Empty<string>();
        }
    }
}
