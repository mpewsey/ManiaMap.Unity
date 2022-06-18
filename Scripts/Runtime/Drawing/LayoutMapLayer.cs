using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class LayoutMapLayer : MonoBehaviour
    {
        public int Z { get; private set; }

        public Texture2D Texture { get; private set; }

        public Sprite Sprite { get; private set; }

        public LayoutMap LayoutMap { get; private set; }

        public SpriteRenderer SpriteRenderer { get; private set; }

        private void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        public static LayoutMapLayer Create(LayoutMap parent, Vector2Int size, int z)
        {
            var obj = new GameObject("Layout Map Layer");
            obj.transform.SetParent(parent.LayersContainer);

            var layer = obj.AddComponent<LayoutMapLayer>();
            layer.LayoutMap = parent;
            layer.Z = z;
            layer.Texture = new Texture2D(size.x, size.y);
            layer.Texture.name = "Mania Map Layer Texture";
            layer.Sprite = layer.CreateSprite();
            layer.SpriteRenderer.sprite = layer.Sprite;

            return layer;
        }

        public void Resize(Vector2Int size)
        {
            if (Texture.width != size.x || Texture.height != size.y)
            {
                Texture.Reinitialize(size.x, size.y);
                Sprite = CreateSprite();
            }

            SpriteRenderer.sprite = Sprite;
        }

        private Sprite CreateSprite()
        {
            var pivot = new Vector2(0.5f, 0.5f);
            var rect = new Rect(0, 0, Texture.width, Texture.height);
            var pixelsPerUnit = LayoutMap.MapTiles.PixelsPerUnit;
            var sprite = Sprite.Create(Texture, rect, pivot, pixelsPerUnit);
            sprite.name = "Mania Map Layer Sprite";
            return sprite;
        }
    }
}
