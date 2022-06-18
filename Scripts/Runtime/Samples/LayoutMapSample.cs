using MPewsey.ManiaMap.Unity.Drawing;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Samples
{
    public class LayoutMapSample : MonoBehaviour
    {
        [SerializeField]
        private int _seed;
        public int Seed { get => _seed; set => _seed = value; }
        
        [SerializeField]
        private GenerationPipeline _pipeline;
        public GenerationPipeline Pipeline { get => _pipeline; set => _pipeline = value; }

        [SerializeField]
        private LayoutMap _layoutMap;
        public LayoutMap LayoutMap { get => _layoutMap; set => _layoutMap = value; }

        private void Start()
        {
            CreateLayers();
        }

        public void CreateLayers()
        {
            var results = Pipeline.Generate(Seed);
            var layout = (Layout)results.Outputs["Layout"];
            var layers = LayoutMap.CreateLayers(layout);
            layers.ForEach(x => x.transform.localPosition = new Vector3(0, 0, x.Z));
        }
    }
}