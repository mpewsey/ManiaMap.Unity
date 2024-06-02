using MPewsey.ManiaMap;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Drawing
{
    /// <summary>
    /// A component for creating maps of Layout layers.
    /// </summary>
    public class LayoutMapBook : LayoutMapBase
    {
        [SerializeField]
        private Color32 _backgroundColor = Color.clear;
        public Color32 BackgroundColor { get => _backgroundColor; set => _backgroundColor = value; }

        [SerializeField]
        private Padding _padding = new Padding(1);
        public Padding Padding { get => _padding; set => _padding = value; }

        [SerializeField]
        private Transform _container;
        public Transform Container { get => _container; set => _container = value; }

        private RectangleInt LayoutBounds { get; set; }
        private List<SpriteRenderer> Pages { get; } = new List<SpriteRenderer>();
        private List<int> PageLayerCoordinates { get; set; } = new List<int>();

        private void Awake()
        {
            if (Container == null)
                Container = transform;
        }

        protected override void Initialize(Layout layout, LayoutState layoutState)
        {
            base.Initialize(layout, layoutState);
            PageLayerCoordinates = RoomsByLayer.Keys.OrderBy(x => x).ToList();
            LayoutBounds = layout.GetBounds();
        }

        public void SaveImages(string path)
        {
            var extension = Path.GetExtension(path);
            var name = Path.ChangeExtension(path, null);

            for (int i = 0; i < Pages.Count; i++)
            {
                var texture = Pages[i].sprite.texture;
                var bytes = TextureUtility.EncodeToBytes(texture, extension);
                File.WriteAllBytes($"{name}_Z={PageLayerCoordinates[i]}{extension}", bytes);
            }
        }

        public void DrawPages(Layout layout, LayoutState layoutState = null)
        {
            Initialize(layout, layoutState);
            SizePages();

            for (int i = 0; i < Pages.Count; i++)
            {
                var texture = Pages[i].sprite.texture;
                DrawTiles(texture, PageLayerCoordinates[i], LayoutBounds, Padding, BackgroundColor);
            }
        }

        private void SizePages()
        {
            while (Pages.Count > PageLayerCoordinates.Count)
            {
                var index = Pages.Count - 1;
                Destroy(Pages[index].gameObject);
                Pages.RemoveAt(index);
            }

            foreach (var page in Pages)
            {
                SizeTexture(page);
            }

            while (Pages.Count < PageLayerCoordinates.Count)
            {
                var obj = new GameObject($"Map {Pages.Count}");
                obj.transform.SetParent(Container);
                var spriteRenderer = obj.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = CreateSprite(CreateTexture());
                Pages.Add(spriteRenderer);
            }
        }

        private void SizeTexture(SpriteRenderer spriteRenderer)
        {
            var size = GetTextureSize();
            var texture = spriteRenderer.sprite.texture;

            if (texture.width != size.x || texture.height != size.y)
            {
                texture.Reinitialize(size.x, size.y);
                spriteRenderer.sprite = CreateSprite(texture);
            }
        }

        private Texture2D CreateTexture()
        {
            var size = GetTextureSize();
            var texture = new Texture2D(size.x, size.y);
            texture.filterMode = MapTileSet.FilterMode;
            return texture;
        }

        private Sprite CreateSprite(Texture2D texture)
        {
            var pivot = new Vector2(0.5f, 0.5f);
            var rect = new Rect(0, 0, texture.width, texture.height);
            var pixelsPerUnit = MapTileSet.PixelsPerUnit;
            var sprite = Sprite.Create(texture, rect, pivot, pixelsPerUnit);
            return sprite;
        }

        private Vector2Int GetTextureSize()
        {
            var width = MapTileSet.TileSize.x * (Padding.Left + Padding.Right + LayoutBounds.Width);
            var height = MapTileSet.TileSize.y * (Padding.Top + Padding.Bottom + LayoutBounds.Height);
            return new Vector2Int(width, height);
        }

        public float SetOnionMapColors(float z, Gradient gradient, float drawDepth = 1)
        {
            if (PageLayerCoordinates.Count > 0)
            {
                var scale = 0.5f / drawDepth;
                var minZ = PageLayerCoordinates[0];
                var maxZ = PageLayerCoordinates[PageLayerCoordinates.Count - 1];
                z = Mathf.Clamp(z, minZ, maxZ);

                for (int i = 0; i < PageLayerCoordinates.Count; i++)
                {
                    var t = (PageLayerCoordinates[i] - z) * scale + 0.5f;
                    Pages[i].color = gradient.Evaluate(Mathf.Clamp(t, 0, 1));
                }
            }

            return z;
        }
    }
}
