using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public abstract class GenerationStep : MonoBehaviour
    {
        public abstract string[] InputNames();
        public abstract string[] OutputNames();
        public abstract IGenerationStep GetStep();
    }
}
