using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Exceptions
{
    public class UnassignedCollectableGroupException : Exception
    {
        public UnassignedCollectableGroupException(string message) : base(message)
        {

        }
    }
}
