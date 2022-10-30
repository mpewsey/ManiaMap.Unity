using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    /// <summary>
    /// A layer of a LayoutMap.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class LayoutMapLayer : MonoBehaviour
    {
        /// <summary>
        /// The layer coordinate.
        /// </summary>
        public int Z { get; private set; }

        /// <summary>
        /// The layer sprite.
        /// </summary>
        public Sprite Sprite { get; private set; }

        /// <summary>
        /// The parent layout map.
        /// </summary>
        public LayoutMap LayoutMap { get; private set; }

        /// <summary>
        /// The attached sprite renderer component.
        /// </summary>
        public SpriteRenderer SpriteRenderer { get; private set; }

        private void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Initializes the layer.
        /// </summary>
        /// <param name="size">The image size.</param>
        /// <param name="z">The layer value.</param>
        public void Initialize(Vector2Int size, int z)
        {
            name = $"Layout Map Layer {z}";
            Z = z;
            ResizeTexture(size);
        }

        /// <summary>
        /// Creates a new layout map layer and returns the result.
        /// </summary>
        /// <param name="map">The parent layout map.</param>
        public static LayoutMapLayer Create(LayoutMap map)
        {
            var obj = new GameObject("Layout Map Layer");
            obj.transform.SetParent(map.LayersContainer);

            var layer = obj.AddComponent<LayoutMapLayer>();
            layer.LayoutMap = map;

            return layer;
        }

        /// <summary>
        /// If the layer sprite does not exist, creates it and assigns it to the renderer.
        /// Otherwise, if the texture size does not match the specified size, reinitializes
        /// it and the sprite.
        /// </summary>
        /// <param name="size">The texture size.</param>
        private void ResizeTexture(Vector2Int size)
        {
            if (Sprite == null)
            {
                var texture = new Texture2D(size.x, size.y);
                texture.name = "Mania Map Layer Texture";
                Sprite = CreateSprite(texture);
                SpriteRenderer.sprite = Sprite;
            }
            else if (Sprite.texture.width != size.x || Sprite.texture.height != size.y)
            {
                Sprite.texture.Reinitialize(size.x, size.y);
                Sprite = CreateSprite(Sprite.texture);
                SpriteRenderer.sprite = Sprite;
            }
        }

        /// <summary>
        /// Creates a new sprite for the specified texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        private Sprite CreateSprite(Texture2D texture)
        {
            var pivot = new Vector2(0.5f, 0.5f);
            var rect = new Rect(0, 0, texture.width, texture.height);
            var pixelsPerUnit = LayoutMap.MapTiles.PixelsPerUnit;
            var sprite = Sprite.Create(texture, rect, pivot, pixelsPerUnit);
            sprite.name = "Mania Map Layer Sprite";
            return sprite;
        }
    }
}
