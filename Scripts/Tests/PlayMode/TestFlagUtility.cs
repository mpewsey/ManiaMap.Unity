using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MPewsey.ManiaMap.Unity
{
    public class TestFlagUtility
    {
        [Test]
        public void TestGetFlagEnumerable()
        {
            var flags = 0;
            var expected = new List<int> { 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 28, 1 << 15, 1 << 30 };
            
            foreach (var flag in expected)
            {
                flags |= flag;
            }

            var result = FlagUtility.GetFlagEnumerable(flags).ToList();
            CollectionAssert.AreEquivalent(expected, result);
        }
    }
}