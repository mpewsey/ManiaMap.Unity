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
        private List<RoomTemplate> _templates = new List<RoomTemplate>();
        /// <summary>
        /// A list of room templates belonging to the group.
        /// </summary>
        public List<RoomTemplate> Templates { get => _templates; set => _templates = value; }

        /// <summary>
        /// Returns an enumerable of loaded room templates in the group.
        /// </summary>
        public IEnumerable<ManiaMap.RoomTemplate> GetTemplates()
        {
            foreach (var template in Templates)
            {
                yield return template.Template;
            }
        }
    }
}
