using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    /// <summary>
    /// The row of the RoomComponent active cells list.
    /// </summary>
    [System.Serializable]
    public struct ActiveCellsRow
    {
        [SerializeField]
        private List<bool> _values;
        /// <summary>
        /// The cell activities for the row.
        /// </summary>
        public List<bool> Values { get => _values; set => _values = value; }

        /// <summary>
        /// Initializes a new struct.
        /// </summary>
        /// <param name="values">The list of values.</param>
        public ActiveCellsRow(List<bool> values)
        {
            _values = values;
        }

        /// <summary>
        /// Initializes a new struct.
        /// </summary>
        /// <param name="count">The number of elements in the row.</param>
        /// <param name="value">The value to assign to each element of the row.</param>
        public ActiveCellsRow(int count, bool value)
        {
            _values = Enumerable.Repeat(value, count).ToList();
        }
    }
}