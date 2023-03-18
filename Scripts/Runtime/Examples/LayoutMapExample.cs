using MPewsey.ManiaMap.Unity.Drawing;
using MPewsey.ManiaMap.Unity.Generators;
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
        /// The random seed.
        /// </summary>
        public int Seed { get => _seed; set => _seed = value; }

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

        private void Start()
        {
            CreateLayers(Seed);
        }

        private void Update()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                CreateLayers();
            }
        }

        /// <summary>
        /// Runs the pipeline and creates a map.
        /// </summary>
        public void CreateLayers()
        {
            CreateLayers(Random.Range(1, int.MaxValue));
        }

        /// <summary>
        /// Runs the pipeline and creates a map for the specified seed.
        /// </summary>
        /// <param name="seed">The random seed.</param>
        public void CreateLayers(int seed)
        {
            Pipeline.SetSeed(seed);
            var results = Pipeline.Generate();

            if (!results.Success)
            {
                Debug.LogError("Failed to generate layout.");
                return;
            }

            var layout = (Layout)results.Outputs["Layout"];
            LayoutMap.CreateLayers(layout, null);
        }
    }
}