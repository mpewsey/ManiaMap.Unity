using MPewsey.Common.Pipelines;
using MPewsey.Common.Random;
using MPewsey.ManiaMap.Unity.Exceptions;
using System.Collections.Generic;
using System.Threading;
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

        [SerializeField]
        private List<string> _manualInputNames = new List<string>();
        /// <summary>
        /// An array of manual input names passed to the generate function.
        /// </summary>
        public List<string> ManualInputNames { get => _manualInputNames; set => _manualInputNames = value; }

        /// <summary>
        /// Generates a set of results for the pipeline.
        /// </summary>
        /// <param name="inputs">A dictionary of manually specified pipeline inputs.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public PipelineResults Run(Dictionary<string, object> inputs = null, CancellationToken cancellationToken = default)
        {
            inputs ??= new Dictionary<string, object>();
            return BuildPipeline().Run(BuildInputs(inputs), cancellationToken);
        }

        /// <summary>
        /// Generates a set of results for the pipeline asynchronously.
        /// </summary>
        /// <param name="inputs">A dictionary of manually specified pipeline inputs.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<PipelineResults> RunAsync(Dictionary<string, object> inputs = null, CancellationToken cancellationToken = default)
        {
            inputs ??= new Dictionary<string, object>();
            return BuildPipeline().RunAsync(BuildInputs(inputs), cancellationToken);
        }

        /// <summary>
        /// Attempts runs of the pipeline until a result is found.
        /// On each failed attempt, the seed is shifted in an attempt to find one that works within the attempt timeout.
        /// </summary>
        /// <param name="seed">The random seed.</param>
        /// <param name="attempts">The maximum number of attempts.</param>
        /// <param name="timeout">The timeout for each attempt.</param>
        /// <param name="inputs">A dictionary of manually specified pipeline inputs.</param>
        public async Task<PipelineResults> RunAttemptsAsync(int seed, int attempts = 10, int timeout = 5000, Dictionary<string, object> inputs = null)
        {
            for (int i = 0; i < attempts; i++)
            {
                Common.Logging.Logger.Log($"[Generation Pipeline] Beginning attempt {i + 1} / {attempts}...");

                var args = new Dictionary<string, object>(inputs)
                {
                    { "RandomSeed", new RandomSeed(seed + i * 1447) },
                };

                var token = new CancellationTokenSource(timeout).Token;
                var results = await RunAsync(args, token);

                if (results.Success)
                {
                    Common.Logging.Logger.Log("[Generation Pipeline] Attempt successful.");
                    return results;
                }
            }

            Common.Logging.Logger.Log("[Generation Pipeline] Generation failed for all attempts.");
            return new PipelineResults(inputs);
        }

        /// <summary>
        /// Returns an array of generation steps attached to the steps container.
        /// </summary>
        public GenerationStep[] GetSteps()
        {
            return StepsContainer.GetComponents<GenerationStep>();
        }

        /// <summary>
        /// Returns an array of generation inputs attached to the inputs container.
        /// </summary>
        public GenerationInput[] GetInputs()
        {
            return InputsContainer.GetComponents<GenerationInput>();
        }

        /// <summary>
        /// Builds the pipeline from the generation steps.
        /// </summary>
        public Pipeline BuildPipeline()
        {
            var steps = GetSteps();
            var pipelineSteps = new IPipelineStep[steps.Length];

            for (int i = 0; i < steps.Length; i++)
            {
                pipelineSteps[i] = steps[i].GetStep();
            }

            return new Pipeline(pipelineSteps);
        }

        /// <summary>
        /// Validates and builts a dictionary of inputs for the pipeline.
        /// </summary>
        public Dictionary<string, object> BuildInputs(Dictionary<string, object> inputs)
        {
            Validate(inputs);
            var result = new Dictionary<string, object>(inputs);

            foreach (var input in GetInputs())
            {
                input.AddInputs(result);
            }

            return result;
        }

        /// <summary>
        /// Validates the pipeline and throws any applicable exceptions.
        /// </summary>
        /// <param name="inputs">A dictionary of manual inputs.</param>
        public void Validate(Dictionary<string, object> inputs)
        {
            var names = new HashSet<string>(inputs.Keys);
            ValidateInputs(names);
            ValidateSteps(names);
        }

        /// <summary>
        /// Validates the pipeline and throws any applicable exceptions.
        /// </summary>
        public void Validate()
        {
            var names = new HashSet<string>(ManualInputNames);
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
            var inputs = GetInputs();

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
            var steps = GetSteps();

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
        /// <param name="inputs">A dictionary of manual inputs.</param>
        public bool IsValid(Dictionary<string, object> inputs)
        {
            var names = new HashSet<string>(inputs.Keys);

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

        /// <summary>
        /// Returns true if the pipeline is valid.
        /// </summary>
        public bool IsValid()
        {
            var names = new HashSet<string>(ManualInputNames);

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