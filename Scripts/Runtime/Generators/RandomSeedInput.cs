using MPewsey.Common.Random;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Generators
{
    /// <summary>
    /// A generation input for specifying a random seed.
    /// </summary>
    public class RandomSeedInput : GenerationInput
    {
        [SerializeField]
        private int _seed = -1;
        /// <summary>
        /// The random seed. If the value is less than or equal to zero, a random value
        /// will be used.
        /// </summary>
        public int Seed { get => _seed; set => _seed = Mathf.Clamp(value, -1, int.MaxValue); }

        /// <summary>
        /// Returns a new random seed.
        /// </summary>
        private RandomSeed GetRandomSeed()
        {
            var seed = Seed <= 0 ? Random.Range(1, int.MaxValue) : Seed;
            return new RandomSeed(seed);
        }

        /// <inheritdoc/>
        public override void AddInputs(Dictionary<string, object> input)
        {
            input.Add("RandomSeed", GetRandomSeed());
        }

        /// <inheritdoc/>
        public override string[] OutputNames()
        {
            return new string[] { "RandomSeed" };
        }

        private void OnValidate()
        {
            Seed = Seed;
        }
    }
}
