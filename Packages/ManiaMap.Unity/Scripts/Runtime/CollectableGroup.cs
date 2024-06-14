using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// A class for creating groups of CollectableResource.
    /// 
    /// The following are some design patterns to consider when implementing collectables
    /// so that they may be referenced by this class:
    /// 1. If the collectables in your project are Scriptable Object and have low memory usage,
    ///    simply have your collectables inherit from the CollectableResource class. The objects
    ///    themselves may then be be directly added to the CollectableGroup.
    /// 2. If the collectables in your project are other object types or have large memory usage,
    ///    create CollectableResource Scriptable Objects in your project that will serve as the collectable unique ID's.
    ///    Reference these collectables in your objects (so that the ID's are synced),
    ///    as well as add the CollectableResource Scriptable Objects to the CollectableGroup.
    /// </summary>
    [CreateAssetMenu(menuName = "Mania Map/Collectable Group")]
    public class CollectableGroup : ScriptableObject
    {
        [SerializeField]
        private string _name = "<None>";
        /// <summary>
        /// The group name.
        /// </summary>
        public string Name { get => _name; set => _name = value; }

        [SerializeField]
        private List<CollectableGroupEntry> _collectables = new List<CollectableGroupEntry>();
        /// <summary>
        /// A list of collectables.
        /// </summary>
        public List<CollectableGroupEntry> Collectables { get => _collectables; set => _collectables = value; }

        /// <summary>
        /// Returns a list of collectable ID's in the quantity specified by the entries.
        /// </summary>
        public List<int> GetCollectableIds()
        {
            var result = new List<int>();

            foreach (var entry in Collectables)
            {
                for (int i = 0; i < entry.Quantity; i++)
                {
                    result.Add(entry.Collectable.Id);
                }
            }

            return result;
        }

        /// <summary>
        /// Adds the collectable to the group if it is valid and doesn't already exist.
        /// Returns true if it is added.
        /// </summary>
        /// <param name="collectable">The collectable.</param>
        public bool AddCollectable(CollectableResource collectable)
        {
            if (collectable != null && !ContainsCollectable(collectable))
            {
                Collectables.Add(new CollectableGroupEntry(collectable, 0));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the group contains the collectable.
        /// </summary>
        /// <param name="collectable">The collectable.</param>
        public bool ContainsCollectable(CollectableResource collectable)
        {
            foreach (var entry in Collectables)
            {
                if (entry.Collectable == collectable)
                    return true;
            }

            return false;
        }
    }
}