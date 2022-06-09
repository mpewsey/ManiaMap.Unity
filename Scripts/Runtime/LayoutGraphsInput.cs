using System;
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
            input.Add("TemplateGroups", GetTemplateGroups());
        }

        public List<System.Func<ManiaMap.LayoutGraph>> GetLayoutGraphFunctions()
        {
            var funcs = new List<System.Func<ManiaMap.LayoutGraph>>(LayoutGraphs.Count);

            foreach (var graph in LayoutGraphs)
            {
                funcs.Add(graph.GetLayoutGraph);
            }

            return funcs;
        }

        public HashSet<TemplateGroup> GetTemplateGroupSet()
        {
            var groups = new HashSet<TemplateGroup>();

            foreach (var graph in LayoutGraphs)
            {
                foreach (var node in graph.GetNodes())
                {
                    groups.Add(node.TemplateGroup);
                }

                foreach (var edge in graph.GetEdges())
                {
                    if (edge.TemplateGroup != null)
                        groups.Add(edge.TemplateGroup);
                }
            }

            return groups;
        }

        public TemplateGroups GetTemplateGroups()
        {
            var groups = new TemplateGroups();
            var pool = new Dictionary<TextAsset, RoomTemplate>();
            var names = new HashSet<string>();

            foreach (var group in GetTemplateGroupSet())
            {
                if (!names.Add(group.Name))
                    throw new ArgumentException($"Duplicate group name: {group.Name}.");
                groups.Add(group.Name, group.LoadTemplates(pool));
            }

            return groups;
        }

        public override string[] OutputNames()
        {
            return new string[] { "LayoutGraphs", "TemplateGroups" };
        }
    }
}
