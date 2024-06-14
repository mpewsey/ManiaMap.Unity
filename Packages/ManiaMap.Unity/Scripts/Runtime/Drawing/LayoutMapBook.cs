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
        /// <summary>
        /// The page background color.
        /// </summary>
        public Color32 BackgroundColor { get => _backgroundColor; set => _backgroundColor = value; }

        [SerializeField]
        private Padding _padding = new Padding(1);
        /// <summary>
        /// The border padding applied to each page.
        /// </summary>
        public Padding Padding { get => _padding; set => _padding = value; }

        [SerializeField]
        private Transform _container;
        /// <summary>
        /// The container used for newly created pages. If not assigned, this object will be used.
        /// </summary>
        public Transform Container { get => _container; set => _container = value; }

        /// <summary>
        /// A list of pages representing layers of the map.
        /// </summary>
        private List<SpriteRenderer> Pages { get; } = new List<SpriteRenderer>();

        /// <summary>
        /// A list of layer (z) coordiantes corresponding to the pages at the same indices.
        /// </summary>
        private List<int> PageLayerCoordinates { get; set; } = new List<int>();

        /// <summary>
        /// A list of pages representing layers of the map.
        /// </summary>
        public IReadOnlyList<SpriteRenderer> GetPages() => Pages;

        /// <summary>
        /// A list of layer (z) coordiantes corresponding to the pages at the same indices.
        /// </summary>
        public IReadOnlyList<int> GetPageLayerCoordinates() => PageLayerCoordinates;

        private void Awake()
        {
            if (Container == null)
                Container = transform;
        }

        /// <summary>
        /// Saves the current pages of the layout to the specified base file path.
        /// Returns a list of resulting layer file paths.
        /// 
        /// The file path for each layer will be constructed as: {BasePathWithoutExtension}_Z={LayerCoordinate}.{BasePathExtension}
        /// </summary>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public List<string> SaveImages(string basePath)
        {
            var result = new List<string>(Pages.Count);
            var extension = Path.GetExtension(basePath);
            var name = Path.ChangeExtension(basePath, null);

            for (int i = 0; i < Pages.Count; i++)
            {
                var texture = Pages[i].sprite.texture;
                var bytes = TextureUtility.EncodeToBytes(texture, extension);
                var savePath = $"{name}_Z={PageLayerCoordinates[i]}{extension}";
                File.WriteAllBytes(savePath, bytes);
                result.Add(savePath);
            }

            return result;
        }

        /// <summary>
        /// Draws all pages of the Layout.
        /// </summary>
        /// <param name="layoutPack">The layout pack.</param>
        public void DrawPages(LayoutPack layoutPack)
        {
            LayoutPack = layoutPack;
            PageLayerCoordinates = layoutPack.GetLayerCoordinates().OrderBy(x => x).ToList();
            SizePages();

            for (int i = 0; i < Pages.Count; i++)
            {
                var texture = Pages[i].sprite.texture;
                DrawTiles(texture, PageLayerCoordinates[i], Padding, BackgroundColor);
            }
        }

        /// <summary>
        /// Draws all pages of the Layout.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="layoutState">The layout state. If null, the map will be drawn completely visible.</param>
        public void DrawPages(Layout layout, LayoutState layoutState = null)
        {
            layoutState ??= CreateFullyVisibleLayoutState(layout);
            var layoutPack = new LayoutPack(layout, layoutState);
            DrawPages(layoutPack);
        }

        /// <summary>
        /// Creates or destroys the pages until they match the required number for the Layout.
        /// </summary>
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

        /// <summary>
        /// If the sprite renderer's assigned texture does not match the required,
        /// resizes the texture and assigns a new sprite.
        /// </summary>
        /// <param name="spriteRenderer">The sprite renderer.</param>
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

        /// <summary>
        /// Creates a new texture with the required size.
        /// </summary>
        private Texture2D CreateTexture()
        {
            var size = GetTextureSize();
            var texture = new Texture2D(size.x, size.y);
            texture.filterMode = MapTileSet.FilterMode;
            texture.name = "Mania Map Texture";
            return texture;
        }

        /// <summary>
        /// Creates a spite with the specified texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        private Sprite CreateSprite(Texture2D texture)
        {
            var pivot = new Vector2(0.5f, 0.5f);
            var rect = new Rect(0, 0, texture.width, texture.height);
            var pixelsPerUnit = MapTileSet.PixelsPerUnit;
            var sprite = Sprite.Create(texture, rect, pivot, pixelsPerUnit);
            return sprite;
        }

        /// <summary>
        /// Returns the overall texture size required for drawing the map with padding.
        /// </summary>
        private Vector2Int GetTextureSize()
        {
            var width = MapTileSet.TileSize.x * (Padding.Left + Padding.Right + LayoutPack.LayoutBounds.Width);
            var height = MapTileSet.TileSize.y * (Padding.Top + Padding.Bottom + LayoutPack.LayoutBounds.Height);
            return new Vector2Int(width, height);
        }

        /// <summary>
        /// Sets the colors of the pages based on the specified layer (z) position and gradient.
        /// This is useful for displaying map overlays like the pages are drawn on onionskin paper.
        /// Returns the layer (z) position used with possible range clamping applied.
        /// </summary>
        /// <param name="z">The current layer (z) position. This value can be between layers and will be clamped as the end points.</param>
        /// <param name="gradient">The gradient applied. The middle of the gradient (0.5) corresponds to the specified z. Pages with larger layer values receive colors to the right of center, while pages with smaller layer values receive colors to the left of center.</param>
        /// <param name="drawDepth">The layer draw depth. This value corresponds to the layer distance between the middle of the gradient and its ends. Layers beyond this threshold are drawn at the nearest gradient end color.</param>
        public float SetOnionskinColors(float z, Gradient gradient, float drawDepth = 1)
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
