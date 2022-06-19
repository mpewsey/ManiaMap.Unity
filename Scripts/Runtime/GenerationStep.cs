using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// The base class for creating a GenerationPipeline step.
    /// </summary>
    public abstract class GenerationStep : MonoBehaviour
    {
        /// <summary>
        /// Returns an array of argument names required by the step.
        /// These names are used for pipeline validation.
        /// </summary>
        public abstract string[] InputNames();

        /// <summary>
        /// Returns an array of argument names added by the step.
        /// These names are used for pipeline validation.
        /// </summary>
        public abstract string[] OutputNames();

        /// <summary>
        /// Returns the generation step used by the pipeline.
        /// </summary>
        public abstract IGenerationStep GetStep();
    }
}
