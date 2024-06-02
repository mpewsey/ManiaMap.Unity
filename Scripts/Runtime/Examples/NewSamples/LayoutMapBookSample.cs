using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Samples;
using MPewsey.ManiaMapUnity.Drawing;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace MPewsey.ManiaMapUnity.Examples
{
    public class LayoutMapBookSample : MonoBehaviour
    {
        [SerializeField] private Button _generateButton;
        public Button GenerateButton { get => _generateButton; set => _generateButton = value; }

        [SerializeField] private Text _messageLabel;
        public Text MessageLabel { get => _messageLabel; set => _messageLabel = value; }

        private LayoutMapBook Map { get; set; }

        private void Awake()
        {
            Map = GetComponent<LayoutMapBook>();
            GenerateButton.onClick.AddListener(OnGenerateButtonPressed);
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
            var result = await BigLayoutSample.GenerateAsync(seed, cancellationToken: token);
            GenerateButton.interactable = true;

            if (!result.Success)
            {
                MessageLabel.text = $"Generation FAILED (Seed = {seed})";
                return;
            }

            MessageLabel.text = string.Empty;
            var layout = result.GetOutput<Layout>("Layout");
            Map.DrawPages(layout);
        }
    }
}