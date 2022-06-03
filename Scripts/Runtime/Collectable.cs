using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// An object representing a collectable with a unique ID.
    /// </summary>
    [CreateAssetMenu(menuName = "Mania Map/Collectable")]
    public class Collectable : ScriptableObject
    {
        [SerializeField]
        private int _id;
        /// <summary>
        /// The unique ID.
        /// </summary>
        public int Id { get => _id; set => _id = value; }

        private void OnValidate()
        {
            AutoAssignId();
        }

        /// <summary>
        /// If the ID is less than or equal to zero, assigns a random positive integer to the ID.
        /// </summary>
        public void AutoAssignId()
        {
            if (Id <= 0)
                Id = Random.Range(1, int.MaxValue);
        }
    }
}
