using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
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
        private List<Entry> _entries = new List<Entry>();
        /// <summary>
        /// A list of template entries.
        /// </summary>
        public List<Entry> Entries { get => _entries; set => _entries = value; }

        private void OnValidate()
        {
            Entries.ForEach(x => x.OnValidate());
        }

        /// <summary>
        /// Returns an enumerable of generation template group entries.
        /// </summary>
        public IEnumerable<TemplateGroups.Entry> GetEntries()
        {
            return Entries.Select(x => x.GetEntry());
        }

        /// <summary>
        /// Add an entry for the template to the group if it does not already exist.
        /// </summary>
        /// <param name="template">The room template.</param>
        public void AddTemplate(RoomTemplate template)
        {
            if (template == null)
                return;

            var index = Entries.FindIndex(x => x.Template == template);

            if (index < 0)
                Entries.Add(new Entry(template));
        }

        /// <summary>
        /// A TemplateGroup entry.
        /// </summary>
        [System.Serializable]
        public class Entry
        {
            [SerializeField]
            private RoomTemplate _template;
            /// <summary>
            /// The room template.
            /// </summary>
            public RoomTemplate Template { get => _template; set => _template = value; }

            [SerializeField]
            private int _minQuantity;
            /// <summary>
            /// The minimum number of times this entry is used in a layout.
            /// </summary>
            public int MinQuantity
            {
                get => _minQuantity;
                set => _minQuantity = Mathf.Max(value, 0);
            }

            [SerializeField]
            private int _maxQuantity = int.MaxValue;
            /// <summary>
            /// The maximum number of times this entry is used in a layout.
            /// </summary>
            public int MaxQuantity
            {
                get => _maxQuantity;
                set => _maxQuantity = Mathf.Max(value, 0);
            }

            public void OnValidate()
            {
                MinQuantity = Mathf.Min(MinQuantity, MaxQuantity);
                MaxQuantity = Mathf.Max(MinQuantity, MaxQuantity);
            }

            /// <summary>
            /// Initializes a new entry with no quantity constraints.
            /// </summary>
            /// <param name="template">The room template.</param>
            public Entry(RoomTemplate template)
            {
                Template = template;
            }

            /// <summary>
            /// Initializes a new entry with quantity constraints.
            /// </summary>
            /// <param name="template">The room template.</param>
            /// <param name="minQuantity">The minimum use quantity</param>
            /// <param name="maxQuantity">The maximum use quantity.</param>
            public Entry(RoomTemplate template, int minQuantity, int maxQuantity)
            {
                Template = template;
                MinQuantity = minQuantity;
                MaxQuantity = maxQuantity;
            }

            /// <summary>
            /// Returns a new generation template group entry.
            /// </summary>
            public TemplateGroups.Entry GetEntry()
            {
                return new TemplateGroups.Entry(Template.Template, MinQuantity, MaxQuantity);
            }
        }
    }
}
