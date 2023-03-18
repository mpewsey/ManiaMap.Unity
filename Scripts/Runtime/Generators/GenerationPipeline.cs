using MPewsey.Common.Pipelines;
using MPewsey.ManiaMap.Unity.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Generators
{
    /// <summary>
    /// A component for generating layouts using a sequence of steps.
    /// </summary>
    public class GenerationPipeline : MonoBehaviour
    {
        [SerializeField]
        private GameObject _inputsContainer;
        /// <summary>
        /// The container containing the GenerationInput components for the pipeline.
        /// </summary>
        public GameObject InputsContainer { get => _inputsContainer; set => _inputsContainer = value; }

        [SerializeField]
        private GameObject _stepsContainer;
        /// <summary>
        /// The container containing the GenerationStep components for the pipeline.
        /// </summary>
        public GameObject StepsContainer { get => _stepsContainer; set => _stepsContainer = value; }

        /// <summary>
        /// Sets the value of the random seed input.
        /// </summary>
        /// <param name="seed">The random seed.</param>
        public void SetSeed(int seed)
        {
            var input = InputsContainer.GetComponent<RandomSeedInput>();
            input.Seed = seed;
        }

        /// <summary>
        /// Sets the value of the layout ID input.
        /// </summary>
        /// <param name="id">The layout ID.</param>
        public void SetLayoutId(int id)
        {
            var input = InputsContainer.GetComponent<LayoutIdInput>();
            input.Id = id;
        }

        /// <summary>
        /// Generates a set of results for the pipeline.
        /// </summary>
        public PipelineResults Generate(Dictionary<string, object> inputs = null)
        {
            inputs ??= new Dictionary<string, object>();
            Validate(inputs);
            return GetPipeline().Generate(GetInputs(inputs));
        }

        /// <summary>
        /// Generates a set of results for the pipeline asynchronously.
        /// </summary>
        public Task<PipelineResults> GenerateAsync(Dictionary<string, object> inputs = null)
        {
            inputs ??= new Dictionary<string, object>();
            Validate(inputs);
            var pipeline = GetPipeline();
            var args = GetInputs(inputs);
            return Task.Run(() => pipeline.Generate(args));
        }

        /// <summary>
        /// Returns an array of generation steps attached to the steps container.
        /// </summary>
        public GenerationStep[] GetGenerationSteps()
        {
            return StepsContainer.GetComponents<GenerationStep>();
        }

        /// <summary>
        /// Returns an array of generation inputs attached to the inputs container.
        /// </summary>
        public GenerationInput[] GetGenerationInputs()
        {
            return InputsContainer.GetComponents<GenerationInput>();
        }

        /// <summary>
        /// Returns the Mania Map generation pipeline.
        /// </summary>
        public Pipeline GetPipeline()
        {
            var steps = GetGenerationSteps();
            var pipelineSteps = new IPipelineStep[steps.Length];

            for (int i = 0; i < steps.Length; i++)
            {
                pipelineSteps[i] = steps[i].GetStep();
            }

            return new Pipeline(pipelineSteps);
        }

        /// <summary>
        /// Returns a dictionary of inputs for the pipeline.
        /// </summary>
        public Dictionary<string, object> GetInputs(Dictionary<string, object> inputs)
        {
            var result = new Dictionary<string, object>(inputs);

            foreach (var input in GetGenerationInputs())
            {
                input.AddInputs(result);
            }

            return result;
        }

        /// <summary>
        /// Validates the pipeline and throws any applicable exceptions.
        /// </summary>
        public void Validate(Dictionary<string, object> inputs = null)
        {
            inputs ??= new Dictionary<string, object>();
            var names = new HashSet<string>(inputs.Keys);
            ValidateInputs(names);
            ValidateSteps(names);
        }

        /// <summary>
        /// Validates the pipeline inputs and throws any applicable exceptions.
        /// </summary>
        /// <param name="names">A set of current input names.</param>
        /// <exception cref="DuplicateInputException">Raised if a duplicate input name exists.</exception>
        private void ValidateInputs(HashSet<string> names)
        {
            var inputs = GetGenerationInputs();

            foreach (var input in inputs)
            {
                foreach (var name in input.OutputNames())
                {
                    if (!names.Add(name))
                        throw new DuplicateInputException($"Duplicate input name: {name} <{input.GetType().Name}>.");
                }
            }
        }

        /// <summary>
        /// Validates the pipeline step inputs and throws any applicable exceptions.
        /// </summary>
        /// <param name="names">A set of current input names.</param>
        /// <exception cref="MissingInputException">Raised if a step is missing an input name.</exception>
        private void ValidateSteps(HashSet<string> names)
        {
            var steps = GetGenerationSteps();

            foreach (var step in steps)
            {
                foreach (var name in step.InputNames())
                {
                    if (!names.Contains(name))
                        throw new MissingInputException($"Missing input name: {name} <{step.GetType().Name}>.");
                }

                foreach (var name in step.OutputNames())
                {
                    names.Add(name);
                }
            }
        }

        /// <summary>
        /// Returns true if the pipeline is valid.
        /// </summary>
        public bool IsValid()
        {
            var names = new HashSet<string>();

            try
            {
                ValidateInputs(names);
            }
            catch (DuplicateInputException)
            {
                return false;
            }

            try
            {
                ValidateSteps(names);
            }
            catch (MissingInputException)
            {
                return false;
            }

            return true;
        }
    }
}