using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public static class Bitwise
    {
        public static IEnumerable<int> GetFlagEnumerable(int flags)
        {
            for (int i = flags; i != 0; i &= i - 1)
            {
                yield return ~(i - 1) & i;
            }
        }
    }
}
