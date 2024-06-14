using MPewsey.Common.Pipelines;
using MPewsey.Common.Random;
using MPewsey.ManiaMapUnity.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Generators
{
    /// <summary>
    /// A component for generating layouts using a sequence of steps.
    /// </summary>
    public class GenerationPipeline : MonoBehaviour
    {
        [SerializeField]
        private List<string> _manualInputNames = new List<string>();
        /// <summary>
        /// An array of manual input names passed to the generate function.
        /// </summary>
        public List<string> ManualInputNames { get => _manualInputNames; set => _manualInputNames = value; }

        /// <summary>
        /// Generates a set of results for the pipeline.
        /// </summary>
        /// <param name="manualInputs">A dictionary of manually specified pipeline inputs.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public PipelineResults Run(Dictionary<string, object> manualInputs = null, System.Action<string> logger = null, CancellationToken cancellationToken = default)
        {
            manualInputs ??= new Dictionary<string, object>();
            return BuildPipeline().Run(BuildInputs(manualInputs), logger, cancellationToken);
        }

        /// <summary>
        /// Generates a set of results for the pipeline asynchronously.
        /// </summary>
        /// <param name="manualInputs">A dictionary of manually specified pipeline inputs.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<PipelineResults> RunAsync(Dictionary<string, object> manualInputs = null, System.Action<string> logger = null, CancellationToken cancellationToken = default)
        {
            manualInputs ??= new Dictionary<string, object>();
            return BuildPipeline().RunAsync(BuildInputs(manualInputs), logger, cancellationToken);
        }

        /// <summary>
        /// Attempts runs of the pipeline until a result is found.
        /// On each failed attempt, the seed is shifted in an attempt to find one that works within the attempt timeout.
        /// </summary>
        /// <param name="seed">The random seed.</param>
        /// <param name="attempts">The maximum number of attempts.</param>
        /// <param name="timeout">The timeout for each attempt.</param>
        /// <param name="manualInputs">A dictionary of manually specified pipeline inputs.</param>
        public async Task<PipelineResults> RunAttemptsAsync(int seed, int attempts = 10, int timeout = 5000, Dictionary<string, object> manualInputs = null, System.Action<string> logger = null)
        {
            manualInputs ??= new Dictionary<string, object>();

            for (int i = 0; i < attempts; i++)
            {
                logger?.Invoke($"[Generation Pipeline] Beginning attempt {i + 1} / {attempts}...");
                var inputs = new Dictionary<string, object>(manualInputs);
                inputs.Add("RandomSeed", new RandomSeed(seed + i * 1447));
                var token = new CancellationTokenSource(timeout).Token;
                var results = await RunAsync(inputs, logger, token);

                if (results.Success)
                {
                    logger?.Invoke("[Generation Pipeline] Attempt successful.");
                    return results;
                }
            }

            logger?.Invoke("[Generation Pipeline] Generation failed for all attempts.");
            return new PipelineResults(manualInputs);
        }

        /// <summary>
        /// Returns an array of generation steps attached to the steps container.
        /// </summary>
        public GenerationStep[] FindStepComponents()
        {
            return GetComponentsInChildren<GenerationStep>();
        }

        /// <summary>
        /// Returns an array of generation inputs attached to the inputs container.
        /// </summary>
        public GenerationInput[] FindInputComponents()
        {
            return GetComponentsInChildren<GenerationInput>();
        }

        /// <summary>
        /// Builds the pipeline from the generation steps.
        /// </summary>
        public Pipeline BuildPipeline()
        {
            var steps = FindStepComponents();
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
        /// <param name="manualInputs">A dictionary of manually specified pipeline inputs.</param>
        public Dictionary<string, object> BuildInputs(Dictionary<string, object> manualInputs = null)
        {
            manualInputs ??= new Dictionary<string, object>();
            Validate(manualInputs.Keys);
            var result = new Dictionary<string, object>(manualInputs);

            foreach (var input in FindInputComponents())
            {
                input.AddInputs(result);
            }

            return result;
        }

        /// <summary>
        /// Validates the pipeline inputs and outputs and throws an exception of invalid.
        /// </summary>
        public void Validate()
        {
            Validate(ManualInputNames);
        }

        /// <summary>
        /// Validates the pipeline inputs and outputs and throws an exception of invalid.
        /// </summary>
        /// <param name="manualInputNames">An enumerable of manual input names.</param>
        public void Validate(IEnumerable<string> manualInputNames)
        {
            manualInputNames ??= Enumerable.Empty<string>();
            var names = new HashSet<string>(manualInputNames);
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
            var inputs = FindInputComponents();

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
            var steps = FindStepComponents();

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
        /// Sets the seed for the first RandomSeedInput child.
        /// </summary>
        /// <param name="seed">The random seed.</param>
        /// <exception cref="MissingInputException">Raised if a random seed input is not found.</exception>
        public void SetRandomSeed(int seed)
        {
            var input = GetComponentInChildren<RandomSeedInput>();

            if (input == null)
                throw new MissingInputException("Random seed input not found.");

            input.Seed = seed;
        }
    }
}