using MPewsey.ManiaMap;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Generators
{
    /// <summary>
    /// A generation input for supplying collectable groups to a GenerationPipeline.
    /// </summary>
    public class CollectableGroupsInput : GenerationInput
    {
        [SerializeField]
        private List<CollectableGroup> _collectableGroups = new List<CollectableGroup>();
        /// <summary>
        /// A list of collectable groups.
        /// </summary>
        public List<CollectableGroup> CollectableGroups { get => _collectableGroups; set => _collectableGroups = value; }

        /// <inheritdoc/>
        public override void AddInputs(Dictionary<string, object> input)
        {
            input.Add("CollectableGroups", GetCollectableGroups());
        }

        /// <inheritdoc/>
        public override string[] OutputNames()
        {
            return new string[] { "CollectableGroups" };
        }

        /// <summary>
        /// Returns the collectable groups for the input.
        /// </summary>
        public CollectableGroups GetCollectableGroups()
        {
            var groups = new CollectableGroups();

            foreach (var group in CollectableGroups)
            {
                groups.Add(group.Name, group.GetCollectableIds());
            }

            return groups;
        }
    }
}
