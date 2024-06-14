using MPewsey.ManiaMap;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MPewsey.ManiaMapUnity.Drawing
{
    /// <summary>
    /// A component for drawing multiple layers of a Layout onto a Tilemap.
    /// </summary>
    public class LayoutTileMapBook : LayoutMapBase
    {
        [SerializeField]
        private Grid _grid;
        /// <summary>
        /// The tilemap grid. If null, one will be created as a child of this object.
        /// </summary>
        public Grid Grid { get => _grid; set => _grid = value; }

        /// <summary>
        /// A list of pages representing layers of the map.
        /// </summary>
        private List<Tilemap> Pages { get; } = new List<Tilemap>();

        /// <summary>
        /// A list of layer (z) coordiantes corresponding to the pages at the same indices.
        /// </summary>
        private List<int> PageLayerCoordinates { get; set; } = new List<int>();

        /// <summary>
        /// Returns a list of the current pages, representing layers, in the map.
        /// </summary>
        public IReadOnlyList<Tilemap> GetPages() => Pages;

        /// <summary>
        /// A list of layer (z) coordiantes corresponding to the pages at the same indices.
        /// </summary>
        public IReadOnlyList<int> GetPageLayerCoordinates() => PageLayerCoordinates;

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
                SetTiles(Pages[i], PageLayerCoordinates[i]);
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
        /// Creates or destroys the tilemap pages until it matches the number required for the layout.
        /// Populates the tilemaps into the Pages list.
        /// </summary>
        protected void SizePages()
        {
            CreateGrid();

            while (Pages.Count > PageLayerCoordinates.Count)
            {
                var index = Pages.Count - 1;
                Destroy(Pages[index].gameObject);
                Pages.RemoveAt(index);
            }

            while (Pages.Count < PageLayerCoordinates.Count)
            {
                var obj = new GameObject($"Tile Map {Pages.Count}");
                obj.transform.SetParent(Grid.transform);
                Pages.Add(obj.AddComponent<Tilemap>());
                obj.AddComponent<TilemapRenderer>();
            }
        }

        /// <summary>
        /// If it doesn't already exist, creates a new Grid as a child and assigns it to the object.
        /// </summary>
        public void CreateGrid()
        {
            if (Grid == null)
            {
                var obj = new GameObject("Grid");
                obj.transform.SetParent(transform);
                Grid = obj.AddComponent<Grid>();
            }
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