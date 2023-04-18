using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    public interface IOnionMapTarget
    {
        public Vector2 LayerRange();
        public IEnumerable<IOnionMapLayer> Layers();
    }
}