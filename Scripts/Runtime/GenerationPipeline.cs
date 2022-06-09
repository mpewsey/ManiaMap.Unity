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
    }
}