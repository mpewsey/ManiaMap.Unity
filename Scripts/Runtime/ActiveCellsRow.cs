using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    [System.Serializable]
    public struct ActiveCellsRow
    {
        [SerializeField]
        private List<bool> _values;
        public List<bool> Values { get => _values; set => _values = value; }

        public ActiveCellsRow(List<bool> values)
        {
            _values = values;
        }

        public ActiveCellsRow(int count, bool value)
        {
            _values = Enumerable.Repeat(value, count).ToList();
        }
    }
}