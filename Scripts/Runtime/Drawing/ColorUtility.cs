using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    public static class ColorUtility
    {
        /// <summary>
        /// Converts a System.Drawing.Color to a Unity Color32.
        /// </summary>
        /// <param name="color">The System.Drawing.Color</param>
        public static Color32 ConvertColor(System.Drawing.Color color)
        {
            return new Color32(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// Calculates the composite of top color A onto bottom color B.
        /// </summary>
        /// <param name="colorA">The top color.</param>
        /// <param name="colorB">The bottom color.</param>
        public static Color CompositeColors(Color colorA, Color colorB)
        {
            var alpha1 = colorA.a;
            var alpha2 = colorB.a * (1 - colorA.a);
            var alpha = alpha1 + alpha2;
            alpha1 /= alpha;
            alpha2 /= alpha;
            var red = colorA.r * alpha1 + colorB.r * alpha2;
            var green = colorA.g * alpha1 + colorB.g * alpha2;
            var blue = colorA.b * alpha1 + colorB.b * alpha2;
            return new Color(red, green, blue, alpha);
        }
    }
}