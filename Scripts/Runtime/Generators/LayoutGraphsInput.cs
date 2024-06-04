using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Generators;
using MPewsey.ManiaMapUnity.Graphs;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Generators
{
    /// <summary>
    /// A generation input for supplying layout graphs inputs to a pipeline.
    /// </summary>
    public class LayoutGraphsInput : GenerationInput
    {
        [SerializeField]
        private List<LayoutGraphResource> _layoutGraphs = new List<LayoutGraphResource>();
        /// <summary>
        /// A list of layout graphs.
        /// </summary>
        public List<LayoutGraphResource> LayoutGraphs { get => _layoutGraphs; set => _layoutGraphs = value; }

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
        public List<LayoutGraphSelector.LayoutGraphDelegate> GetLayoutGraphFunctions()
        {
            var funcs = new List<LayoutGraphSelector.LayoutGraphDelegate>(LayoutGraphs.Count);

            foreach (var graph in LayoutGraphs)
            {
                funcs.Add(graph.CreateData);
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
                groups.Add(group.Name, group.GetMMTemplateGroupEntries());
            }

            return groups;
        }
    }
}
