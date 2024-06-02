using MPewsey.ManiaMap;
using MPewsey.ManiaMapUnity.Drawing;
using MPewsey.ManiaMapUnity.Generators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Examples
{
    /// <summary>
    /// A component for initializing the LayoutTilemap samples.
    /// </summary>
    public class LayoutTilemapExample : MonoBehaviour
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
        private GenerationPipeline _pipeline;
        /// <summary>
        /// The generation pipeline.
        /// </summary>
        public GenerationPipeline Pipeline { get => _pipeline; set => _pipeline = value; }

        [SerializeField]
        private LayoutTileMap _layoutTilemap;
        /// <summary>
        /// The layout tilemap.
        /// </summary>
        public LayoutTileMap LayoutTilemap { get => _layoutTilemap; set => _layoutTilemap = value; }

        /// <summary>
        /// True if the generation task is currently running.
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
            };

            TaskIsRunning = true;
            var task = Pipeline.RunAttemptsAsync(seed, 10, 5000, inputs);

            while (!task.IsCompleted)
            {
                yield return null;
            }

            TaskIsRunning = false;

            if (!task.Result.Success)
                throw new System.TimeoutException("Task timed out.");

            var layout = task.Result.GetOutput<Layout>("Layout");
            LayoutTilemap.DrawMap(layout);
        }
    }
}