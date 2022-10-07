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
            CreateLayers();
        }

        /// <summary>
        /// Runs the pipeline and creates the map.
        /// </summary>
        public void CreateLayers()
        {
            Pipeline.SetSeed(Seed);
            var results = Pipeline.Generate();
            var layout = (Layout)results.Outputs["Layout"];
            var layers = LayoutTilemap.CreateLayers(layout);
            layers.ForEach(x => x.transform.localPosition = new Vector3(0, 0, x.Z));
        }
    }
}