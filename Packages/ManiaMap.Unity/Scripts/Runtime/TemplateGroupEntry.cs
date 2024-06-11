using MPewsey.ManiaMap;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// A TemplateGroup entry.
    /// </summary>
    [System.Serializable]
    public class TemplateGroupEntry
    {
        [SerializeField]
        private RoomTemplateResource _template;
        /// <summary>
        /// The room template.
        /// </summary>
        public RoomTemplateResource Template { get => _template; set => _template = value; }

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
        public TemplateGroupEntry(RoomTemplateResource template)
        {
            Template = template;
        }

        /// <summary>
        /// Initializes a new entry with quantity constraints.
        /// </summary>
        /// <param name="template">The room template.</param>
        /// <param name="minQuantity">The minimum use quantity</param>
        /// <param name="maxQuantity">The maximum use quantity.</param>
        public TemplateGroupEntry(RoomTemplateResource template, int minQuantity, int maxQuantity)
        {
            Template = template;
            MinQuantity = minQuantity;
            MaxQuantity = maxQuantity;
        }

        /// <summary>
        /// Returns a new generation template group entry.
        /// </summary>
        public TemplateGroupsEntry GetMMTemplateGroupsEntry(Dictionary<RoomTemplateResource, RoomTemplate> templateCache)
        {
            if (!templateCache.TryGetValue(Template, out var template))
            {
                template = Template.GetMMRoomTemplate();
                templateCache.Add(Template, template);
            }

            return new TemplateGroupsEntry(template, MinQuantity, MaxQuantity);
        }
    }
}