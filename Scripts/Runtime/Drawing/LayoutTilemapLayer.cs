using UnityEngine;
using UnityEngine.Tilemaps;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    [RequireComponent(typeof(Tilemap))]
    [RequireComponent(typeof(TilemapRenderer))]
    public class LayoutTilemapLayer : MonoBehaviour
    {
        public int Z { get; private set; }

        public Tilemap Tilemap { get; private set; }

        public LayoutTilemap LayoutTilemap { get; private set; }

        private void Awake()
        {
            Tilemap = GetComponent<Tilemap>();
            LayoutTilemap = GetComponentInParent<LayoutTilemap>();
        }

        public static LayoutTilemapLayer Create(int z, Transform parent)
        {
            var obj = new GameObject("Tilemap Layer");
            obj.transform.SetParent(parent);

            var layer = obj.AddComponent<LayoutTilemapLayer>();
            layer.Z = z;

            return layer;
        }
    }
}