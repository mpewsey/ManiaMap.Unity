using MPewsey.ManiaMap.Unity.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class GenerationPipeline : MonoBehaviour
    {
        [SerializeField]
        private GameObject _inputsContainer;
        public GameObject InputsContainer { get => _inputsContainer; set => _inputsContainer = value; }

        [SerializeField]
        private GameObject _stepsContainer;
        public GameObject StepsContainer { get => _stepsContainer; set => _stepsContainer = value; }

        public ManiaMap.GenerationPipeline.Results Generate()
        {
            var seed = Random.Range(1, int.MaxValue);
            return Generate(seed);
        }

        public ManiaMap.GenerationPipeline.Results Generate(int seed)
        {
            Validate();
            var pipeline = GetPipeline();
            var inputs = GetInputs(seed);
            return pipeline.Generate(inputs);
        }

        public Task<ManiaMap.GenerationPipeline.Results> GenerateAsync()
        {
            var seed = Random.Range(1, int.MaxValue);
            return GenerateAsync(seed);
        }

        public Task<ManiaMap.GenerationPipeline.Results> GenerateAsync(int seed)
        {
            Validate();
            var pipeline = GetPipeline();
            var inputs = GetInputs(seed);
            return Task.Run(() => pipeline.Generate(inputs));
        }

        public GenerationStep[] GetGenerationSteps()
        {
            return StepsContainer.GetComponents<GenerationStep>();
        }

        public GenerationInput[] GetGenerationInputs()
        {
            return InputsContainer.GetComponents<GenerationInput>();
        }

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

        public Dictionary<string, object> GetInputs(int seed)
        {
            var inputs = GetGenerationInputs();
            var result = new Dictionary<string, object>()
            {
                { "RandomSeed", new RandomSeed(seed) },
            };

            foreach (var input in inputs)
            {
                input.AddInput(result);
            }

            return result;
        }

        public void Validate()
        {
            var names = new HashSet<string>() { "RandomSeed" };
            ValidateInputs(names);
            ValidateSteps(names);
        }

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

        public bool IsValid()
        {
            var names = new HashSet<string>() { "RandomSeed" };

            return GenerationInputsAreValid(names)
                && GenerationStepsAreValid(names);
        }

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