using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public abstract class GenerationStep : MonoBehaviour
    {
        public abstract IGenerationStep GetStep();
    }
}
