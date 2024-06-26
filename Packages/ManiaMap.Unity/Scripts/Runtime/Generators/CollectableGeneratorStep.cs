using MPewsey.Common.Pipelines;
using MPewsey.ManiaMap.Generators;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Generators
{
    /// <summary>
    /// A component for adding a collectable generator step to a GenerationPipeline.
    /// </summary>
    public class CollectableGeneratorStep : GenerationStep
    {
        [SerializeField]
        private float _doorPower = 2;
        /// <summary>
        /// The exponent power applied to the door weight.
        /// </summary>
        public float DoorPower { get => _doorPower; set => _doorPower = value; }

        [SerializeField]
        private float _neighborPower = 1;
        /// <summary>
        /// The exponent power applied to the neighbor weight.
        /// </summary>
        public float NeighborPower { get => _neighborPower; set => _neighborPower = value; }

        [SerializeField]
        private int _initialNeighborWeight = 1000;
        /// <summary>
        /// The initial weight used for collectable spots with no allocated neighbors.
        /// </summary>
        public int InitialNeighborWeight { get => _initialNeighborWeight; set => _initialNeighborWeight = value; }

        /// <inheritdoc/>
        public override IPipelineStep GetStep()
        {
            return new CollectableGenerator(DoorPower, NeighborPower, InitialNeighborWeight);
        }

        /// <inheritdoc/>
        public override string[] InputNames()
        {
            return new string[] { "Layout", "CollectableGroups", "RandomSeed" };
        }

        /// <inheritdoc/>
        public override string[] OutputNames()
        {
            return System.Array.Empty<string>();
        }
    }
}
