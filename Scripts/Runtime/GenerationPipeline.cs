using MPewsey.ManiaMap.Unity.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
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

        public void SetSeed(int seed)
        {
            var input = InputsContainer.GetComponent<RandomSeedInput>();
            input.Seed = seed;
        }

        /// <summary>
        /// Generates a set of results for the pipeline.
        /// </summary>
        public ManiaMap.GenerationPipeline.Results Generate()
        {
            Validate();
            var pipeline = GetPipeline();
            var inputs = GetInputs();
            return pipeline.Generate(inputs);
        }

        /// <summary>
        /// Generates a set of results for the pipeline asynchronously.
        /// </summary>
        public Task<ManiaMap.GenerationPipeline.Results> GenerateAsync()
        {
            Validate();
            var pipeline = GetPipeline();
            var inputs = GetInputs();
            return Task.Run(() => pipeline.Generate(inputs));
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
        public ManiaMap.GenerationPipeline GetPipeline()
        {
            var pipeline = new ManiaMap.GenerationPipeline();
            var steps = GetGenerationSteps();

            foreach (var step in steps)
            {
                pipeline.Steps.Add(step.GetStep());
            }

            return pipeline;
        }

        /// <summary>
        /// Returns a dictionary of inputs for the pipeline.
        /// </summary>
        public Dictionary<string, object> GetInputs()
        {
            var inputs = GetGenerationInputs();
            var result = new Dictionary<string, object>();

            foreach (var input in inputs)
            {
                input.AddInput(result);
            }

            return result;
        }

        /// <summary>
        /// Validates the pipeline and throws any applicable exceptions.
        /// </summary>
        public void Validate()
        {
            var names = new HashSet<string>();
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

            return GenerationInputsAreValid(names)
                && GenerationStepsAreValid(names);
        }

        /// <summary>
        /// Returns true if the pipeline inputs are valid.
        /// </summary>
        /// <param name="names">A set of current input names.</param>
        private bool GenerationInputsAreValid(HashSet<string> names)
        {
            var inputs = GetGenerationInputs();

            foreach (var input in inputs)
            {
                foreach (var name in input.OutputNames())
                {
                    if (!names.Add(name))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true if the pipeline step inputs are valid.
        /// </summary>
        /// <param name="names">A set of current input names.</param>
        private bool GenerationStepsAreValid(HashSet<string> names)
        {
            var steps = GetGenerationSteps();

            foreach (var step in steps)
            {
                foreach (var name in step.InputNames())
                {
                    if (!names.Contains(name))
                        return false;
                }

                foreach (var name in step.OutputNames())
                {
                    names.Add(name);
                }
            }

            return true;
        }
    }
}