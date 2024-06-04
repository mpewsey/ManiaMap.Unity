using MPewsey.Common.Random;
using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Generators;
using MPewsey.ManiaMap.Graphs;
using MPewsey.ManiaMap.Samples;
using MPewsey.ManiaMapUnity.Drawing;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Examples
{
    public class OnionMapExample : MonoBehaviour
    {
        [SerializeField]
        private int _seed = 12345;
        /// <summary>
        /// The random seed.
        /// </summary>
        public int Seed { get => _seed; set => _seed = value; }

        [SerializeField]
        private float _speed = 10;
        public float Speed { get => _speed; set => _speed = value; }

        [SerializeField]
        private KeyCode _increaseKey = KeyCode.Alpha1;
        public KeyCode IncreaseKey { get => _increaseKey; set => _increaseKey = value; }

        [SerializeField]
        private KeyCode _decreaseKey = KeyCode.Alpha2;
        public KeyCode DecreaseKey { get => _decreaseKey; set => _decreaseKey = value; }

        [SerializeField]
        private Gradient _gradient;
        public Gradient Gradient { get => _gradient; set => _gradient = value; }

        [SerializeField]
        private LayoutTileMapBook _layoutTilemap;
        /// <summary>
        /// The layout tilemap.
        /// </summary>
        public LayoutTileMapBook LayoutTilemap { get => _layoutTilemap; set => _layoutTilemap = value; }

        private float Z { get; set; }

        private void Start()
        {
            Draw();
        }

        private void Update()
        {
            var direction = Input.GetKey(IncreaseKey) ? 1 : Input.GetKey(DecreaseKey) ? -1 : 0;
            Z = LayoutTilemap.SetOnionMapColors(Z + Speed * Time.deltaTime * direction, Gradient);
        }

        public void Draw()
        {
            LayoutTilemap.DrawPages(Layout());
        }

        private Layout Layout()
        {
            var graph = LayoutGraph();
            var random = new RandomSeed(Seed);
            var generator = new LayoutGenerator();
            var templateGroups = new TemplateGroups();
            templateGroups.Add("Default", TemplateLibrary.Miscellaneous.HyperSquareTemplate());
            return generator.Generate(1, graph, templateGroups, random);
        }

        private static LayoutGraph LayoutGraph()
        {
            var graph = new LayoutGraph(1, "StackedLoopLayoutGraph");
            graph.AddNode(0).SetZ(1);
            graph.AddNode(1).SetZ(1);
            graph.AddNode(2).SetZ(1);
            graph.AddNode(3).SetZ(-1);
            graph.AddEdge(0, 1);
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);
            graph.AddEdge(4, 0);
            graph.AddEdge(0, 5);
            graph.AddEdge(5, 6);
            graph.AddEdge(6, 7);
            graph.AddEdge(7, 3);
            return graph;
        }
    }
}