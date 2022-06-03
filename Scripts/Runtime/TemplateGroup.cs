using System.Collections.Generic;
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
        private List<TextAsset> _templates = new List<TextAsset>();
        /// <summary>
        /// A list of serialized room templates belonging to the group.
        /// </summary>
        public List<TextAsset> Templates { get => _templates; set => _templates = value; }

        /// <summary>
        /// Loads the room templates in the group and returns a list.
        /// </summary>
        public List<RoomTemplate> LoadTemplates()
        {
            var templates = new List<RoomTemplate>(Templates.Count);

            foreach (var asset in Templates)
            {
                var template = Serialization.LoadXml<RoomTemplate>(asset.bytes);
                templates.Add(template);
            }

            return templates;
        }
    }
}
