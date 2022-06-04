using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class LayoutGraphRandomizer : GenerationStep
    {
        public override IGenerationStep GetStep()
        {
            return new ManiaMap.LayoutGraphRandomizer();
        }
    }
}