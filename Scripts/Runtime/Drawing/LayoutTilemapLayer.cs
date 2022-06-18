using UnityEngine;
using UnityEngine.Tilemaps;

namespace MPewsey.ManiaMap.Unity.Drawing
{
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

        public static LayoutTilemapLayer Create(LayoutTilemap parent, int z)
        {
            var obj = new GameObject("Tilemap Layer");
            obj.transform.SetParent(parent.Grid.transform);

            var layer = obj.AddComponent<LayoutTilemapLayer>();
            layer.LayoutTilemap = parent;
            layer.Z = z;

            return layer;
        }
    }
}