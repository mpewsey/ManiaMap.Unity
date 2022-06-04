using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public abstract class GenerationInput : MonoBehaviour
    {
        public abstract void AddInput(Dictionary<string, object> input);
    }
}
