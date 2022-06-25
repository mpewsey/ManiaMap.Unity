using MPewsey.ManiaMap.Unity.Drawing;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Samples
{
    /// <summary>
    /// A component for initializing the LayoutMap samples.
    /// </summary>
    public class LayoutMapSample : MonoBehaviour
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
        private LayoutMap _layoutMap;
        /// <summary>
        /// The layout map.
        /// </summary>
        public LayoutMap LayoutMap { get => _layoutMap; set => _layoutMap = value; }

        private void Start()
        {
            CreateLayers();
        }

        /// <summary>
        /// Runs the pipeline and creates the map.
        /// </summary>
        public void CreateLayers()
        {
            var results = Pipeline.Generate(Seed);
            var layout = (Layout)results.Outputs["Layout"];
            var layers = LayoutMap.CreateLayers(layout);
            layers.ForEach(x => x.transform.localPosition = new Vector3(0, 0, x.Z));
        }
    }
}