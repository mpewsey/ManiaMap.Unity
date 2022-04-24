using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    [CreateAssetMenu(menuName = "Mania Map/Template Group")]
    public class TemplateGroup : ScriptableObject
    {
        [SerializeField]
        private string _name = "<None>";
        public string Name { get => _name; set => _name = value; }
    }
}
