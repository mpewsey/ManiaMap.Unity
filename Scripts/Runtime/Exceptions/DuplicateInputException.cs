using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Exceptions
{
    public class DuplicateInputException : Exception
    {
        public DuplicateInputException(string message) : base(message)
        {

        }
    }
}
