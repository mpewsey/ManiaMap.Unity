using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// An object representing a collectable with a unique ID.
    /// </summary>
    [CreateAssetMenu(menuName = "Mania Map/Collectable")]
    public class CollectableResource : ScriptableObject
    {
        [SerializeField]
        private bool _editId;
        /// <summary>
        /// If true, the ID can be edited in the inspector.
        /// </summary>
        public bool EditId { get => _editId; set => _editId = value; }

        [SerializeField]
        private int _id = -1;
        /// <summary>
        /// The unique ID.
        /// </summary>
        public int Id { get => _id; set => _id = value; }

        protected virtual void OnValidate()
        {
            Id = Rand.AutoAssignId(Id);
        }
    }
}
