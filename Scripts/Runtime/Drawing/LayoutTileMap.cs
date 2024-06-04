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

        public void DrawMap(int? z = null)
        {
            var manager = ManiaMapManager.Current;
            DrawMap(manager.Layout, manager.LayoutState, z);
        }

        public void DrawMap(Layout layout, LayoutState layoutState = null, int? z = null)
        {
            Initialize(layout, layoutState);
            CreateTileMap();
            LayerCoordinate = z ?? RoomsByLayer.Keys.OrderBy(x => x).First();
            SetTiles(TileMap, LayerCoordinate);
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