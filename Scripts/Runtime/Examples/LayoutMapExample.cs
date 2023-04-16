using MPewsey.Common.Random;
using MPewsey.ManiaMap.Unity.Drawing;
using MPewsey.ManiaMap.Unity.Generators;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Examples
{
    /// <summary>
    /// A component for initializing the LayoutMap samples.
    /// </summary>
    public class LayoutMapExample : MonoBehaviour
    {
        [SerializeField]
        private int _seed = 12345;
        /// <summary>
        /// The initial random seed.
        /// </summary>
        public int Seed { get => _seed; set => _seed = value; }

        [SerializeField]
        private string _generateButton = "Fire1";
        /// <summary>
        /// The button pressed to generate a new layout.
        /// </summary>
        public string GenerateButton { get => _generateButton; set => _generateButton = value; }

        [SerializeField]
        private int _timeout = 10000;
        /// <summary>
        /// The timeout in milliseconds.
        /// </summary>
        public int Timeout { get => _timeout; set => _timeout = value; }

        [SerializeField]
        private GenerationPipeline _pipeline;
        /// <summary>
        /// The generation pipeline.
        /// </summary>
        public GenerationPipeline Pipeline { get => _pipeline; set => _pipeline = value; }

        [SerializeField]
        private LayoutMapBehavior _layoutMap;
        /// <summary>
        /// The layout map.
        /// </summary>
        public LayoutMapBehavior LayoutMap { get => _layoutMap; set => _layoutMap = value; }

        /// <summary>
        /// True if the generation task is running.
        /// </summary>
        public bool TaskIsRunning { get; private set; }

        private void Start()
        {
            StartCoroutine(DrawCoroutine(Seed));
        }

        private void Update()
        {
            if (!TaskIsRunning && Input.GetButtonDown(GenerateButton))
                StartCoroutine(DrawCoroutine());
        }

        /// <summary>
        /// Runs the pipeline and creates a map.
        /// </summary>
        public IEnumerator DrawCoroutine()
        {
            return DrawCoroutine(Random.Range(1, int.MaxValue));
        }

        /// <summary>
        /// Runs the pipeline and creates a map for the specified seed.
        /// </summary>
        /// <param name="seed">The random seed.</param>
        public IEnumerator DrawCoroutine(int seed)
        {
            var inputs = new Dictionary<string, object>()
            {
                { "LayoutId", 1 },
                { "RandomSeed", new RandomSeed(seed) },
            };

            LayoutMap.Clear();
            TaskIsRunning = true;
            var task = Pipeline.GenerateAsync(inputs);
            var tasks = Task.WhenAny(task, Task.Delay(Timeout));

            while (!tasks.IsCompleted)
            {
                yield return null;
            }

            TaskIsRunning = false;

            if (!task.IsCompletedSuccessfully)
                throw new System.TimeoutException("Task timed out.");
            if (!task.Result.Success)
                throw new System.Exception("Failed to generate layout.");

            var layout = task.Result.GetOutput<Layout>("Layout");
            LayoutMap.Initialize(layout);
            LayoutMap.Draw();
        }
    }
}