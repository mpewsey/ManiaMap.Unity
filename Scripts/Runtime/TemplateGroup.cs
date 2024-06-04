using MPewsey.ManiaMap;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// A class for grouping room templates.
    /// </summary>
    [CreateAssetMenu(menuName = "Mania Map/Template Group")]
    public class TemplateGroup : ScriptableObject
    {
        [SerializeField]
        private string _name = "<None>";
        /// <summary>
        /// The unique group name.
        /// </summary>
        public string Name { get => _name; set => _name = value; }

        [SerializeField]
        private List<TemplateGroupEntry> _entries = new List<TemplateGroupEntry>();
        /// <summary>
        /// A list of template entries.
        /// </summary>
        public List<TemplateGroupEntry> Entries { get => _entries; set => _entries = value; }

        private void OnValidate()
        {
            foreach (var entry in Entries)
            {
                entry.OnValidate();
            }
        }

        /// <summary>
        /// Returns a list of generation template group entries.
        /// </summary>
        public List<TemplateGroupsEntry> CreateData()
        {
            var result = new List<TemplateGroupsEntry>(Entries.Count);

            foreach (var entry in Entries)
            {
                result.Add(entry.CreateData());
            }

            return result;
        }

        /// <summary>
        /// Add an entry for the template to the group if it does not already exist.
        /// </summary>
        /// <param name="template">The room template.</param>
        public void AddTemplate(RoomTemplateObject template)
        {
            if (template != null && !ContainsTemplate(template))
                Entries.Add(new TemplateGroupEntry(template));
        }

        /// <summary>
        /// Returns true if the template is contained in the group.
        /// </summary>
        /// <param name="template">The template.</param>
        private bool ContainsTemplate(RoomTemplateObject template)
        {
            foreach (var entry in Entries)
            {
                if (entry.Template == template)
                    return true;
            }

            return false;
        }
    }
}
