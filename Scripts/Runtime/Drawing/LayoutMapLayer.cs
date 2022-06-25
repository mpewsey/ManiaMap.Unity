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
        /// Creates a new layout map layer and returns the result.
        /// </summary>
        /// <param name="map">The parent layout map.</param>
        /// <param name="size">The size of the texture.</param>
        /// <param name="z">The layer.</param>
        public static LayoutMapLayer Create(LayoutMap map, Vector2Int size, int z)
        {
            var obj = new GameObject($"Layout Map Layer {z}");
            obj.transform.SetParent(map.LayersContainer);

            var layer = obj.AddComponent<LayoutMapLayer>();
            layer.LayoutMap = map;
            layer.Z = z;
            layer.Resize(size);

            return layer;
        }

        /// <summary>
        /// If the layer sprite does not exist, creates it and assigns it to the renderer.
        /// Otherwise, if the texture size does not match the specified size, reinitializes
        /// it and the sprite.
        /// </summary>
        /// <param name="size">The texture size.</param>
        public void Resize(Vector2Int size)
        {
            if (Sprite == null)
            {
                var texture = new Texture2D(size.x, size.y);
                texture.name = $"Mania Map Layer {Z} Texture";
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
            sprite.name = $"Mania Map Layer {Z} Sprite";
            return sprite;
        }
    }
}
