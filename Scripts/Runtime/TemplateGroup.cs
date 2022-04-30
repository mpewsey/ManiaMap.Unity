using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    [CreateAssetMenu(menuName = "Mania Map/Template Group")]
    public class TemplateGroup : ScriptableObject
    {
        [SerializeField]
        private string _name = "<None>";
        public string Name { get => _name; set => _name = value; }

        [SerializeField]
        private List<TextAsset> _templates = new List<TextAsset>();
        public List<TextAsset> Templates { get => _templates; set => _templates = value; }

        public List<ManiaMap.RoomTemplate> LoadTemplates()
        {
            var templates = new List<ManiaMap.RoomTemplate>(Templates.Count);

            foreach (var asset in Templates)
            {
                templates.Add(LoadTemplate(asset));
            }

            return templates;
        }

        private static ManiaMap.RoomTemplate LoadTemplate(TextAsset asset)
        {
            var serializer = new DataContractSerializer(typeof(ManiaMap.RoomTemplate));

            using (var stream = new MemoryStream(asset.bytes))
            {
                return (ManiaMap.RoomTemplate)serializer.ReadObject(stream);
            }
        }
    }
}
