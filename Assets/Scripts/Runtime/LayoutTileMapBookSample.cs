using MPewsey.Common.Pipelines;
using MPewsey.Common.Random;
using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Generators;
using MPewsey.ManiaMap.Samples;
using MPewsey.ManiaMapUnity.Drawing;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MPewsey.ManiaMapUnity.Examples
{
    [RequireComponent(typeof(LayoutTileMapBook))]
    public class LayoutTileMapBookSample : MonoBehaviour
    {
        [SerializeField] private Button _generateButton;
        public Button GenerateButton { get => _generateButton; set => _generateButton = value; }

        [SerializeField] private Text _messageLabel;
        public Text MessageLabel { get => _messageLabel; set => _messageLabel = value; }

        [SerializeField] private Slider _slider;
        public Slider Slider { get => _slider; set => _slider = value; }

        [SerializeField] private GameObject _sliderContainer;
        public GameObject SliderContainer { get => _sliderContainer; set => _sliderContainer = value; }

        [SerializeField] private Text _zLabel;
        public Text ZLabel { get => _zLabel; set => _zLabel = value; }

        [SerializeField] private Gradient _gradient = Gradients.RedWhiteBlueGradient();
        public Gradient Gradient { get => _gradient; set => _gradient = value; }

        [SerializeField] private Camera2DController _camera;
        public Camera2DController Camera { get => _camera; set => _camera = value; }

        private LayoutTileMapBook Map { get; set; }

        private void Awake()
        {
            Map = GetComponent<LayoutTileMapBook>();
            GenerateButton.onClick.AddListener(OnGenerateButtonPressed);
            Slider.onValueChanged.AddListener(OnSliderValueChanged);
            SliderContainer.SetActive(false);
        }

        private void OnSliderValueChanged(float value)
        {
            Map.SetOnionMapColors(value, Gradient);
            ZLabel.text = value.ToString("0.00");
        }

        private void OnGenerateButtonPressed()
        {
            GenerateMapAsync();
        }

        private async void GenerateMapAsync()
        {
            MessageLabel.text = "Generating...";
            GenerateButton.interactable = false;
            var seed = Random.Range(1, int.MaxValue);
            var token = new CancellationTokenSource(10000).Token;
            var result = await GenerateLayout(seed);
            GenerateButton.interactable = true;

            if (!result.Success)
            {
                MessageLabel.text = $"Generation FAILED (Seed = {seed})";
                return;
            }

            SliderContainer.SetActive(true);
            MessageLabel.text = string.Empty;
            var layout = result.GetOutput<Layout>("Layout");
            Map.DrawPages(layout);

            var zs = Map.GetPageLayerCoordinates();
            SliderContainer.SetActive(true);
            Slider.minValue = zs[0];
            Slider.maxValue = zs[zs.Count - 1];
            Slider.value = Mathf.FloorToInt((Slider.minValue + Slider.maxValue) * 0.5f);
            OnSliderValueChanged(Slider.value);
            Camera.ResetPosition();
        }

        private static Task<PipelineResults> GenerateLayout(int seed)
        {
            var templateGroups = new TemplateGroups();
            templateGroups.Add("Default", TemplateLibrary.Miscellaneous.HyperSquareTemplate());

            var inputs = new Dictionary<string, object>()
            {
                { "LayoutId", 1 },
                { "RandomSeed", new RandomSeed(seed) },
                { "LayoutGraph", GraphLibrary.StackedLoopGraph() },
                { "TemplateGroups", templateGroups },
            };

            var pipeline = new Pipeline(new LayoutGenerator());
            return pipeline.RunAsync(inputs);
        }
    }
}