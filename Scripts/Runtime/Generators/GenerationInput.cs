using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Generators
{
    /// <summary>
    /// The base class for a GenerationPipeline input.
    /// </summary>
    public abstract class GenerationInput : MonoBehaviour
    {
        /// <summary>
        /// An array of argument names that this input adds to the inputs dictionary.
        /// These names are used for pipeline validation.
        /// </summary>
        public abstract string[] OutputNames();

        /// <summary>
        /// Adds the input arguments to the specified inputs dictionary.
        /// </summary>
        /// <param name="input">The input dictionary.</param>
        public abstract void AddInputs(Dictionary<string, object> input);
    }
}
