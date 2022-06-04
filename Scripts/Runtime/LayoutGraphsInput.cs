using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class LayoutGraphsInput : GenerationInput
    {
        [SerializeField]
        private List<LayoutGraph> _layoutGraphs = new List<LayoutGraph>();
        public List<LayoutGraph> LayoutGraphs { get => _layoutGraphs; set => _layoutGraphs = value; }

        public override void AddInput(Dictionary<string, object> input)
        {
            input.Add("LayoutGraphs", GetLayoutGraphFunctions());
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

        public override string[] OutputNames()
        {
            return new string[] { "LayoutGraphs" };
        }
    }
}
