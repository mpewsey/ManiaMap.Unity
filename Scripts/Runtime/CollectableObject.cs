using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// An object representing a collectable with a unique ID.
    /// </summary>
    [CreateAssetMenu(menuName = "Mania Map/Collectable")]
    public class CollectableObject : ScriptableObject
    {
        [SerializeField]
        private int _id;
        /// <summary>
        /// The unique ID.
        /// </summary>
        public int Id { get => _id; set => _id = value; }

        protected virtual void OnValidate()
        {
            Id = AutoId.AutoAssignId(Id);
        }
    }
}
