using UnityEngine;
using UnityEngine.Tilemaps;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    /// <summary>
    /// A layer of a LayoutTilemap.
    /// </summary>
    [RequireComponent(typeof(Tilemap))]
    [RequireComponent(typeof(TilemapRenderer))]
    public class LayoutTilemapLayer : MonoBehaviour
    {
        /// <summary>
        /// The layer value.
        /// </summary>
        public int Z { get; private set; }

        /// <summary>
        /// The attached tilemap.
        /// </summary>
        public Tilemap Tilemap { get; private set; }

        /// <summary>
        /// The parent layout tilemap.
        /// </summary>
        public LayoutTilemap LayoutTilemap { get; private set; }

        private void Awake()
        {
            Tilemap = GetComponent<Tilemap>();
        }

        /// <summary>
        /// Creates a new layer.
        /// </summary>
        /// <param name="map">The layout tilemap.</param>
        /// <param name="z">The layer coordinate.</param>
        public static LayoutTilemapLayer Create(LayoutTilemap map, int z)
        {
            var obj = new GameObject($"Tilemap Layer {z}");
            obj.transform.SetParent(map.Grid.transform);

            var layer = obj.AddComponent<LayoutTilemapLayer>();
            layer.LayoutTilemap = map;
            layer.Z = z;

            return layer;
        }
    }
}