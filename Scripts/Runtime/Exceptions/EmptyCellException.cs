using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Exceptions
{
    public class EmptyCellException : Exception
    {
        public EmptyCellException(string message) : base(message)
        {

        }
    }
}