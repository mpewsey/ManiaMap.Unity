using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Drawing
{
    /// <summary>
    /// Contains methods for manipulating textures.
    /// </summary>
    public static class TextureUtility
    {
        /// <summary>
        /// Returns the flat array index corresponding to the row-column index.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="columns">The number of columns.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Index(int row, int column, int columns)
        {
            return row * columns + column;
        }

        /// <summary>
        /// Fills the 1 pixel border around the texture with the colors at a 1 pixel inset.
        /// </summary>
        /// <param name="texture">The texture.</param>
        public static void FillBorder(Texture2D texture)
        {
            if (!TextureIsNullOrEmpty(texture) && texture.width > 2 && texture.height > 2)
            {
                var width = texture.width;
                var height = texture.height;
                var pixels = texture.GetRawTextureData<Color32>();

                for (int i = 1; i < height - 1; i++)
                {
                    pixels[Index(i, width - 1, width)] = pixels[Index(i, width - 2, width)];
                    pixels[Index(i, 0, width)] = pixels[Index(i, 1, width)];
                }

                for (int j = 0; j < width; j++)
                {
                    pixels[Index(height - 1, j, width)] = pixels[Index(height - 2, j, width)];
                    pixels[Index(0, j, width)] = pixels[Index(1, j, width)];
                }
            }
        }

        /// <summary>
        /// Fills the texture with the specified color.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="color">The color.</param>
        public static void Fill(Texture2D texture, Color32 color)
        {
            if (!TextureIsNullOrEmpty(texture))
            {
                var pixels = texture.GetRawTextureData<Color32>();

                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = color;
                }
            }
        }

        /// <summary>
        /// Fills the specified texture area with the specified color.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="color">The color.</param>
        /// <param name="area">The area to fill from the bottom left of the texture.</param>
        public static void Fill(Texture2D texture, Color32 color, RectInt area)
        {
            if (!TextureIsNullOrEmpty(texture))
            {
                var pixels = texture.GetRawTextureData<Color32>();
                var textureWidth = texture.width;

                for (int i = 0; i < area.height; i++)
                {
                    var row = i + area.y;

                    for (int j = 0; j < area.width; j++)
                    {
                        var column = j + area.x;
                        pixels[Index(row, column, textureWidth)] = color;
                    }
                }
            }
        }

        /// <summary>
        /// Composites the color onto all pixels of the specified texture.
        /// </summary>
        /// <param name="texture">The tecture.</param>
        /// <param name="color">The color.</param>
        public static void CompositeFill(Texture2D texture, Color color)
        {
            if (!TextureIsNullOrEmpty(texture))
            {
                var pixels = texture.GetRawTextureData<Color32>();

                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = ColorUtility.CompositeColors(color, pixels[i]);
                }
            }
        }

        /// <summary>
        /// Composites the color onto the pixels in the specified texture area.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="color">The color.</param>
        /// <param name="area">The area to fill from the bottom left of the texture.</param>
        public static void CompositeFill(Texture2D texture, Color color, RectInt area)
        {
            if (!TextureIsNullOrEmpty(texture))
            {
                var pixels = texture.GetRawTextureData<Color32>();
                var textureWidth = texture.width;

                for (int i = 0; i < area.height; i++)
                {
                    var row = i + area.y;

                    for (int j = 0; j < area.width; j++)
                    {
                        var column = j + area.x;
                        var index = Index(row, column, textureWidth);
                        pixels[index] = ColorUtility.CompositeColors(color, pixels[index]);
                    }
                }
            }
        }

        /// <summary>
        /// Tiles the image across the texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="brush">The tile texture.</param>
        public static void TileImage(Texture2D texture, Texture2D brush)
        {
            if (!TextureIsNullOrEmpty(texture) && !TextureIsNullOrEmpty(brush))
            {
                var row = 0;
                var brushPixels = brush.GetRawTextureData<Color32>();
                var pixels = texture.GetRawTextureData<Color32>();
                var textureHeight = texture.height;
                var textureWidth = texture.width;
                var brushHeight = brush.height;
                var brushWidth = brush.width;

                for (int i = 0; i < textureHeight; i++)
                {
                    var column = 0;

                    for (int j = 0; j < textureWidth; j++)
                    {
                        var index = Index(i, j, textureWidth);
                        var color = brushPixels[Index(row, column, brushWidth)];
                        pixels[index] = ColorUtility.CompositeColors(color, pixels[index]);

                        if (++column >= brushWidth)
                            column = 0;
                    }

                    if (++row >= brushHeight)
                        row = 0;
                }
            }
        }

        /// <summary>
        /// Draws the brush texture at the specified point.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="brush">The brush texture.</param>
        /// <param name="point">The draw point from the bottom left of the texture.</param>
        public static void DrawImage(Texture2D texture, Texture2D brush, Vector2Int point)
        {
            if (!TextureIsNullOrEmpty(texture) && !TextureIsNullOrEmpty(brush))
            {
                var pixels = texture.GetRawTextureData<Color32>();
                var brushPixels = brush.GetRawTextureData<Color32>();
                var textureWidth = texture.width;
                var brushWidth = brush.width;
                var brushHeight = brush.height;

                for (int i = 0; i < brushHeight; i++)
                {
                    var row = i + point.y;

                    for (int j = 0; j < brushWidth; j++)
                    {
                        var column = j + point.x;
                        var index = Index(row, column, textureWidth);
                        var color = brushPixels[Index(i, j, brushWidth)];
                        pixels[index] = ColorUtility.CompositeColors(color, pixels[index]);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the image format bytes for the texture corresponding to the file extension (.png or .jpg).
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="extension">The file extension.</param>
        /// <exception cref="ArgumentException">Raised if the file extension is not handled.</exception>
        public static byte[] EncodeToBytes(Texture2D texture, string extension)
        {
            switch (extension.TrimStart('.').ToLower())
            {
                case "png":
                    return texture.EncodeToPNG();
                case "jpg":
                case "jpeg":
                    return texture.EncodeToJPG();
                default:
                    throw new ArgumentException($"Unhandled file extension: {extension}");
            }
        }

        /// <summary>
        /// Returns true if the texture is null or an empty array.
        /// </summary>
        /// <param name="texture">The texture.</param>
        public static bool TextureIsNullOrEmpty(Texture2D texture)
        {
            return texture == null || texture.width == 0 || texture.height == 0;
        }
    }
}
