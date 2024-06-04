using MPewsey.ManiaMap;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MPewsey.ManiaMapUnity.Drawing
{
    /// <summary>
    /// A component for creating tilemaps of layout layers.
    /// </summary>
    public class LayoutTileMapBook : LayoutMapBase
    {
        [SerializeField]
        private Grid _grid;
        public Grid Grid { get => _grid; set => _grid = value; }

        private List<Tilemap> Pages { get; } = new List<Tilemap>();
        private List<int> PageLayerCoordinates { get; set; } = new List<int>();

        public IReadOnlyList<Tilemap> GetPages() => Pages;
        public IReadOnlyList<int> GetPageLayerCoordinates() => PageLayerCoordinates;

        protected override void Initialize(Layout layout, LayoutState layoutState)
        {
            base.Initialize(layout, layoutState);
            PageLayerCoordinates = RoomsByLayer.Keys.OrderBy(x => x).ToList();
        }

        public void DrawPages()
        {
            var manager = ManiaMapManager.Current;
            DrawPages(manager.Layout, manager.LayoutState);
        }

        public void DrawPages(Layout layout, LayoutState layoutState = null)
        {
            Initialize(layout, layoutState);
            SizePages();

            for (int i = 0; i < Pages.Count; i++)
            {
                SetTiles(Pages[i], PageLayerCoordinates[i]);
            }
        }

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

        public void CreateGrid()
        {
            if (Grid == null)
            {
                var obj = new GameObject("Grid");
                obj.transform.SetParent(transform);
                Grid = obj.AddComponent<Grid>();
            }
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