using MPewsey.ManiaMap;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MPewsey.ManiaMapUnity.Drawing
{
    public class LayoutTileMap : LayoutMapBase
    {
        [SerializeField]
        private Grid _grid;
        public Grid Grid { get => _grid; set => _grid = value; }

        public Tilemap TileMap { get; private set; }
        public int LayerCoordinate { get; private set; }

        public void DrawMap(LayoutPack layoutPack, int? z = null)
        {
            LayoutPack = layoutPack;
            LayerCoordinate = z ?? layoutPack.GetLayerCoordinates().OrderBy(x => x).First();
            CreateTileMap();
            SetTiles(TileMap, LayerCoordinate);
        }

        public void DrawMap(Layout layout, LayoutState layoutState = null, int? z = null)
        {
            layoutState ??= CreateFullyVisibleLayoutState(layout);
            var layoutPack = new LayoutPack(layout, layoutState);
            DrawMap(layoutPack, z);
        }

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