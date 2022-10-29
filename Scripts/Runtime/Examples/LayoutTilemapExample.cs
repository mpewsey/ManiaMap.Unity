using MPewsey.ManiaMap.Unity.Drawing;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Examples
{
    /// <summary>
    /// A component for initializing the LayoutTilemap samples.
    /// </summary>
    public class LayoutTilemapExample : MonoBehaviour
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
        private LayoutTilemap _layoutTilemap;
        /// <summary>
        /// The layout tilemap.
        /// </summary>
        public LayoutTilemap LayoutTilemap { get => _layoutTilemap; set => _layoutTilemap = value; }

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
            var layers = LayoutTilemap.CreateLayers(layout, null);
            layers.ForEach(x => x.transform.localPosition = new Vector3(0, 0, x.Z));
        }
    }
}