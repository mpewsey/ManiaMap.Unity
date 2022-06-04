using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class LayoutGraphSelector : GenerationStep
    {
        public override IGenerationStep GetStep()
        {
            return new ManiaMap.LayoutGraphSelector();
        }
    }
}
