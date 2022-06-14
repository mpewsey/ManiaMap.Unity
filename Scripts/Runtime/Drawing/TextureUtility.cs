using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    public static class TextureUtility
    {
        public static void Fill(Texture2D texture, Color32 color)
        {
            var pixels = texture.GetRawTextureData<Color32>();

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
        }

        public static void Fill(Texture2D texture, Color32 color, RectInt area)
        {
            var pixels = texture.GetRawTextureData<Color32>();

            for (int i = 0; i < area.height; i++)
            {
                for (int j = 0; j < area.width; j++)
                {
                    var row = i + area.y;
                    var column = j + area.x;
                    var index = row * texture.width + column;
                    pixels[index] = color;
                }
            }
        }

        public static void CompositeFill(Texture2D texture, Color color)
        {
            var pixels = texture.GetRawTextureData<Color32>();

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = ColorUtility.CompositeColors(color, pixels[i]);
            }
        }

        public static void CompositeFill(Texture2D texture, Color color, RectInt area)
        {
            var pixels = texture.GetRawTextureData<Color32>();

            for (int i = 0; i < area.height; i++)
            {
                for (int j = 0; j < area.width; j++)
                {
                    var row = i + area.y;
                    var column = j + area.x;
                    var index = row * texture.width + column;
                    pixels[index] = ColorUtility.CompositeColors(color, pixels[index]);
                }
            }
        }

        public static void TileImage(Texture2D texture, Texture2D brush)
        {
            if (brush != null)
            {
                var brushPixels = brush.GetRawTextureData<Color32>();
                var pixels = texture.GetRawTextureData<Color32>();

                for (int i = 0; i < texture.height; i++)
                {
                    for (int j = 0; j < texture.width; j++)
                    {
                        var row = i % brush.height;
                        var column = j % brush.width;
                        var index = i * texture.width + j;
                        var color = brushPixels[row * brush.width + column];
                        pixels[index] = ColorUtility.CompositeColors(color, pixels[index]);
                    }
                }
            }
        }

        public static void DrawImage(Texture2D texture, Texture2D brush, Vector2Int point)
        {
            if (brush != null)
            {
                var pixels = texture.GetRawTextureData<Color32>();
                var brushPixels = brush.GetRawTextureData<Color32>();

                for (int i = 0; i < brush.height; i++)
                {
                    for (int j = 0; j < brush.width; j++)
                    {
                        var row = i + point.y;
                        var column = j + point.x;
                        var index = row * texture.width + column;
                        var color = brushPixels[i * brush.width + j];
                        pixels[index] = ColorUtility.CompositeColors(color, pixels[index]);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the image format bytes for the texture corresponding to the file extension (.png or .jpg).
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="ext">The file extension.</param>
        /// <exception cref="ArgumentException">Raised if the file extension is not handled.</exception>
        public static byte[] EncodeToBytes(Texture2D texture, string ext)
        {
            switch (ext.ToLower())
            {
                case "png":
                case ".png":
                    return texture.EncodeToPNG();
                case "jpg":
                case ".jpg":
                case "jpeg":
                case ".jpeg":
                    return texture.EncodeToJPG();
                default:
                    throw new ArgumentException($"Unhandled file extension: {ext}");
            }
        }
    }
}
