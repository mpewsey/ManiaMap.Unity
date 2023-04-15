using UnityEngine;
using UnityEngine.Tilemaps;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    /// <summary>
    /// A layer of a LayoutTilemap.
    /// </summary>
    [RequireComponent(typeof(Tilemap))]
    [RequireComponent(typeof(TilemapRenderer))]
    public class LayoutTilemapLayer : MonoBehaviour, IOnionMapLayer
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
        public LayoutTilemapBehavior LayoutTilemap { get; private set; }

        private void Awake()
        {
            Tilemap = GetComponent<Tilemap>();
        }

        float IOnionMapLayer.Position() => Z;

        void IOnionMapLayer.Apply(Color color)
        {
            Tilemap.color = color;
        }

        /// <summary>
        /// Initializes the layer.
        /// </summary>
        /// <param name="z">The layer value.</param>
        public void Initialize(int z)
        {
            name = $"Tilemap Layer {z}";
            Z = z;
        }

        /// <summary>
        /// Creates a new layer.
        /// </summary>
        /// <param name="map">The layout tilemap.</param>
        public static LayoutTilemapLayer Create(LayoutTilemapBehavior map)
        {
            var obj = new GameObject("Tilemap Layer");
            obj.transform.SetParent(map.Grid.transform);
            var layer = obj.AddComponent<LayoutTilemapLayer>();
            layer.LayoutTilemap = map;
            return layer;
        }
    }
}