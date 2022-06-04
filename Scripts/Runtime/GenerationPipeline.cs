using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class GenerationPipeline : MonoBehaviour
    {
        [SerializeField]
        private int _layoutId;
        public int LayoutId { get => _layoutId; set => _layoutId = value; }
        
        [SerializeField]
        private List<LayoutGraph> _layoutGraphs = new List<LayoutGraph>();
        public List<LayoutGraph> LayoutGraphs { get => _layoutGraphs; set => _layoutGraphs = value; }

        [SerializeField]
        private List<TemplateGroup> _templateGroups = new List<TemplateGroup>();
        public List<TemplateGroup> TemplateGroups { get => _templateGroups; set => _templateGroups = value; }

        [SerializeField]
        private List<CollectableGroup> _collectableGroups = new List<CollectableGroup>();
        public List<CollectableGroup> CollectableGroups { get => _collectableGroups; set => _collectableGroups = value; }

        public ManiaMap.GenerationPipeline.Results Generate()
        {
            var seed = Random.Range(0, int.MaxValue);
            return Generate(seed);
        }

        public ManiaMap.GenerationPipeline.Results Generate(int seed)
        {
            var pipeline = GetPipeline();
            var inputs = GetInputs(seed);
            return pipeline.Generate(inputs);
        }

        public ManiaMap.GenerationPipeline GetPipeline()
        {
            var pipeline = new ManiaMap.GenerationPipeline();
            var steps = GetComponentsInChildren<GenerationStep>();

            foreach (var step in steps)
            {
                pipeline.Steps.Add(step.GetStep());
            }

            return pipeline;
        }

        public Dictionary<string, object> GetInputs(int seed)
        {
            var inputs = GetComponentsInChildren<GenerationInput>();
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

        public TemplateGroups GetTemplateGroups()
        {
            var groups = new TemplateGroups();
            var dict = new Dictionary<TextAsset, RoomTemplate>();
            var templates = new List<RoomTemplate>();

            foreach (var group in TemplateGroups)
            {
                templates.Clear();
                
                foreach (var asset in group.Templates)
                {
                    if (!dict.TryGetValue(asset, out RoomTemplate template))
                    {
                        template = Serialization.LoadXml<RoomTemplate>(asset.bytes);
                        dict.Add(asset, template);
                    }

                    templates.Add(template);
                }

                groups.Add(group.Name, templates);
            }

            return groups;
        }

        public CollectableGroups GetCollectableGroups()
        {
            var groups = new CollectableGroups();

            foreach (var group in CollectableGroups)
            {
                groups.Add(group.Name, group.GetCollectableIds());
            }

            return groups;
        }

        public List<System.Func<ManiaMap.LayoutGraph>> GetLayoutGraphFunctions()
        {
            var funcs = new List<System.Func<ManiaMap.LayoutGraph>>(LayoutGraphs.Count);

            foreach (var graph in LayoutGraphs)
            {
                funcs.Add(() => graph.GetLayoutGraph());
            }

            return funcs;
        }
    }
}