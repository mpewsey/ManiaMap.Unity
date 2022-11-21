using System;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A generation input for supplying layout graphs inputs to a pipeline.
    /// </summary>
    public class LayoutGraphsInput : GenerationInput
    {
        [SerializeField]
        private List<LayoutGraph> _layoutGraphs = new List<LayoutGraph>();
        /// <summary>
        /// A list of layout graphs.
        /// </summary>
        public List<LayoutGraph> LayoutGraphs { get => _layoutGraphs; set => _layoutGraphs = value; }

        /// <inheritdoc/>
        public override void AddInputs(Dictionary<string, object> input)
        {
            input.Add("LayoutGraphs", GetLayoutGraphFunctions());
            input.Add("TemplateGroups", GetTemplateGroups());
        }

        /// <inheritdoc/>
        public override string[] OutputNames()
        {
            return new string[] { "LayoutGraphs", "TemplateGroups" };
        }

        /// <summary>
        /// Returns a list of generation layout graph creation functions for the
        /// supplied layout graphs.
        /// </summary>
        public List<Func<Graphs.LayoutGraph>> GetLayoutGraphFunctions()
        {
            var funcs = new List<Func<Graphs.LayoutGraph>>(LayoutGraphs.Count);

            foreach (var graph in LayoutGraphs)
            {
                funcs.Add(graph.GetLayoutGraph);
            }

            return funcs;
        }

        /// <summary>
        /// Returns a new set of template groups contained in all layout graphs.
        /// </summary>
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

        /// <summary>
        /// Returns the template groups for all layout graphs.
        /// </summary>
        /// <exception cref="ArgumentException">Raised if a duplicate group name exists.</exception>
        public TemplateGroups GetTemplateGroups()
        {
            var groups = new TemplateGroups();
            var names = new HashSet<string>();

            foreach (var group in GetTemplateGroupSet())
            {
                if (!names.Add(group.Name))
                    throw new ArgumentException($"Duplicate group name: {group.Name}.");
                groups.Add(group.Name, group.GetEntries());
            }

            return groups;
        }
    }
}
