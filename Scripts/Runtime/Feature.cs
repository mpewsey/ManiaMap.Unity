using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A component for defining a cell feature.
    /// </summary>
    public class Feature : CellChild
    {
        [SerializeField]
        private string _name = "<None>";
        /// <summary>
        /// The feature name. This name should exactly match the feature name
        /// of the corresponding map tile if you plan for it to be rendered on a map.
        /// </summary>
        public string Name { get => _name; set => _name = value; }
    }
}
