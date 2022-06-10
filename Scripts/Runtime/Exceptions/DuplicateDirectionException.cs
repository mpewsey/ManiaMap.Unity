using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Exceptions
{
    public class DuplicateDirectionException : Exception
    {
        public DuplicateDirectionException(string message) : base(message)
        {

        }
    }
}
