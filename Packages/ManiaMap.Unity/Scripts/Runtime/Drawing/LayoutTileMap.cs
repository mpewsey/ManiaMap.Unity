using MPewsey.ManiaMap;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MPewsey.ManiaMapUnity.Drawing
{
    /// <summary>
    /// A component for drawing a Layout onto a Tilemap.
    /// </summary>
    public class LayoutTileMap : LayoutMapBase
    {
        [SerializeField]
        private Grid _grid;
        /// <summary>
        /// The tilemap grid. If null, one will be created as a child of this object.
        /// </summary>
        public Grid Grid { get => _grid; set => _grid = value; }

        /// <summary>
        /// The tilemap.
        /// </summary>
        public Tilemap TileMap { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int LayerCoordinate { get; private set; }

        /// <summary>
        /// Draws the Layout onto the map for the specified layer (z) coordinate.
        /// </summary>
        /// <param name="layoutPack">The layout pack.</param>
        /// <param name="z">The layer (z) coordinate to draw. If null, the first found coordinate in the Layer will be used.</param>
        public void DrawMap(LayoutPack layoutPack, int? z = null)
        {
            LayoutPack = layoutPack;
            LayerCoordinate = z ?? layoutPack.GetLayerCoordinates().OrderBy(x => x).First();
            CreateTileMap();
            SetTiles(TileMap, LayerCoordinate);
        }

        /// <summary>
        /// Draws the Layout onto the map for the specified layer (z) coordinate.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="layoutState">The layout state. If null, the map will be drawn completely visible.</param>
        /// <param name="z">The layer (z) coordinate to draw. If null, the first found coordinate in the Layer will be used.</param>
        public void DrawMap(Layout layout, LayoutState layoutState = null, int? z = null)
        {
            layoutState ??= CreateFullyVisibleLayoutState(layout);
            var layoutPack = new LayoutPack(layout, layoutState);
            DrawMap(layoutPack, z);
        }

        /// <summary>
        /// If it doesn't already exist, creates a Tilemap as a child of the Grid and assigns
        /// it to the object.
        /// </summary>
        private void CreateTileMap()
        {
            CreateGrid();

            if (TileMap == null)
            {
                var obj = new GameObject("TileMap");
                obj.transform.SetParent(Grid.transform);
                TileMap = obj.AddComponent<Tilemap>();
                obj.AddComponent<TilemapRenderer>();
            }
        }

        /// <summary>
        /// If it doesn't already exist, creates the Grid as a child and assigns it to the object.
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
    }
}