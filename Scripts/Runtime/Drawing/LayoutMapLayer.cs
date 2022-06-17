using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    public class LayoutMapLayer : MonoBehaviour
    {
        public int Z { get; private set; }

        public Texture2D Texture { get; private set; }

        public LayoutMap LayoutMap { get; private set; }

        private void Awake()
        {
            LayoutMap = GetComponentInParent<LayoutMap>();
        }

        public static LayoutMapLayer Create(Vector2Int size, int z, Transform parent)
        {
            var obj = new GameObject("Layout Map Layer");
            obj.transform.SetParent(parent);

            var layer = obj.AddComponent<LayoutMapLayer>();
            layer.Z = z;
            layer.Texture = new Texture2D(size.x, size.y);

            return layer;
        }

        public void Resize(Vector2Int size)
        {
            if (Texture.width != size.x || Texture.height != size.y)
                Texture.Reinitialize(size.x, size.y);
        }

        public Sprite GetSprite()
        {
            var pivot = new Vector2(0.5f, 0.5f);
            var rect = new Rect(0, 0, Texture.width, Texture.height);
            var pixelsPerUnit = LayoutMap.MapTiles.PixelsPerUnit;
            return Sprite.Create(Texture, rect, pivot, pixelsPerUnit);
        }
    }
}
