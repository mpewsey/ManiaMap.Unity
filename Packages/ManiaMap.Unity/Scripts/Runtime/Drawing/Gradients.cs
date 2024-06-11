using UnityEngine;

namespace MPewsey.ManiaMapUnity.Drawing
{
    /// <summary>
    /// Contains color gradient creation methods. These are useful for creating onionskin maps.
    /// </summary>
    public static class Gradients
    {
        /// <summary>
        /// Returns a red, white, and blue color gradient.
        /// </summary>
        public static Gradient RedWhiteBlueGradient()
        {
            var gradient = new Gradient();

            var colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(Color.red, 0),
                new GradientColorKey(Color.white, 0.45f),
                new GradientColorKey(Color.white, 0.55f),
                new GradientColorKey(Color.blue, 1),
            };

            var alphaKeys = new GradientAlphaKey[]
            {
                new GradientAlphaKey(0, 0),
                new GradientAlphaKey(1, 0.45f),
                new GradientAlphaKey(1, 0.55f),
                new GradientAlphaKey(0, 1),
            };

            gradient.SetKeys(colorKeys, alphaKeys);
            return gradient;
        }
    }
}