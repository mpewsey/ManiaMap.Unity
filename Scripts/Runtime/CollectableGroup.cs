using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A class for creating groups of Collectable.
    /// 
    /// The following are some design patterns to consider when implementing collectables
    /// so that they may be referenced by this class:
    /// 1. If the collectables in your project are Scriptable Object and have low memory usage,
    ///    simply have your collectables inherit from the Collectable class. The objects
    ///    themselves may then be be directly added to the CollectableGroup.
    /// 2. If the collectables in your project are other object types or have large memory usage,
    ///    create Collectable Scriptable Objects in your project that will serve as the collectable unique ID's.
    ///    Reference these collectables in your objects (so that the ID's are synced),
    ///    as well as add the Collectable Scriptable Objects to the CollectableGroup.
    /// </summary>
    [CreateAssetMenu(menuName = "Mania Map/Collectable Group")]
    public class CollectableGroup : ScriptableObject
    {
        [SerializeField]
        private string _name = "<Name>";
        /// <summary>
        /// The group name.
        /// </summary>
        public string Name { get => _name; set => _name = value; }

        [SerializeField]
        private List<Collectable> _collectables = new List<Collectable>();
        /// <summary>
        /// A list of collectables.
        /// </summary>
        public List<Collectable> Collectables { get => _collectables; set => _collectables = value; }
    }
}