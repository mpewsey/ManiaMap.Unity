using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Exceptions
{
    public class MissingInputException : Exception
    {
        public MissingInputException(string message) : base(message)
        {

        }
    }
}
