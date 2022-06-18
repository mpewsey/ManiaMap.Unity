using MPewsey.ManiaMap.Unity.Drawing;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Samples
{
    public class LayoutTilemapSample : MonoBehaviour
    {
        [SerializeField]
        private int _seed;
        public int Seed { get => _seed; set => _seed = value; }
        
        [SerializeField]
        private GenerationPipeline _pipeline;
        public GenerationPipeline Pipeline { get => _pipeline; set => _pipeline = value; }

        [SerializeField]
        private LayoutTilemap _layoutTilemap;
        public LayoutTilemap LayoutTilemap { get => _layoutTilemap; set => _layoutTilemap = value; }

        private void Start()
        {
            CreateLayers();
        }

        public void CreateLayers()
        {
            var results = Pipeline.Generate(Seed);
            var layout = (Layout)results.Outputs["Layout"];
            var layers =LayoutTilemap.CreateLayers(layout);
            layers.ForEach(x => x.transform.localPosition = new Vector3(0, 0, x.Z));
        }
    }
}