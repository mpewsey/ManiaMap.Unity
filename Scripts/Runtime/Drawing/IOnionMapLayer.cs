using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    public interface IOnionMapLayer
    {
        public float Position();
        public void Apply(Color color);
    }
}