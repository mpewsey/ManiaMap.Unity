using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class CollectableGroupsInput : GenerationInput
    {
        [SerializeField]
        private List<CollectableGroup> _collectableGroups = new List<CollectableGroup>();
        public List<CollectableGroup> CollectableGroups { get => _collectableGroups; set => _collectableGroups = value; }

        public override void AddInput(Dictionary<string, object> input)
        {
            input.Add("CollectableGroups", GetCollectableGroups());
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

        public override string[] OutputNames()
        {
            return new string[] { "CollectableGroups" };
        }
    }
}
