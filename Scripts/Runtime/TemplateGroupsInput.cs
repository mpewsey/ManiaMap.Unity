using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class TemplateGroupsInput : GenerationInput
    {
        [SerializeField]
        private List<TemplateGroup> _templateGroups = new List<TemplateGroup>();
        public List<TemplateGroup> TemplateGroups { get => _templateGroups; set => _templateGroups = value; }

        public override void AddInput(Dictionary<string, object> input)
        {
            input.Add("TemplateGroups", GetTemplateGroups());
        }

        public TemplateGroups GetTemplateGroups()
        {
            var groups = new TemplateGroups();
            var pool = new Dictionary<TextAsset, RoomTemplate>();

            foreach (var group in TemplateGroups)
            {
                groups.Add(group.Name, group.LoadTemplates(pool));
            }

            return groups;
        }

        public override string[] OutputNames()
        {
            return new string[] { "TemplateGroups" };
        }
    }
}
