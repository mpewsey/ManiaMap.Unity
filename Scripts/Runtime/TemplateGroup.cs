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
        /// Returns an enumerable of loaded room templates in the group.
        /// </summary>
        public IEnumerable<RoomTemplate> LoadTemplates()
        {
            foreach (var asset in Templates)
            {
                yield return Serialization.LoadXml<RoomTemplate>(asset.bytes);
            }
        }

        /// <summary>
        /// Returns an enumerable of loaded room templates in the group.
        /// </summary>
        /// <param name="pool">A dictionary of loaded templates that are queried first.</param>
        public IEnumerable<RoomTemplate> LoadTemplates(Dictionary<TextAsset, RoomTemplate> pool)
        {
            foreach (var asset in Templates)
            {
                if (!pool.TryGetValue(asset, out RoomTemplate template))
                {
                    template = Serialization.LoadXml<RoomTemplate>(asset.bytes);
                    pool.Add(asset, template);
                }

                yield return template;
            }
        }
    }
}
