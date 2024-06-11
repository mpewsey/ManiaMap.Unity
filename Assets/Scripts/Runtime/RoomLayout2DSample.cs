using MPewsey.ManiaMap;
using MPewsey.ManiaMapUnity.Generators;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace MPewsey.ManiaMapUnity.Examples
{
    public class RoomLayout2DSample : MonoBehaviour
    {
        [SerializeField] private GenerationPipeline _pipeline;
        public GenerationPipeline Pipeline { get => _pipeline; set => _pipeline = value; }

        [SerializeField] private RoomTemplateDatabase _roomTemplateDatabase;
        public RoomTemplateDatabase RoomTemplateDatabase { get => _roomTemplateDatabase; set => _roomTemplateDatabase = value; }

        [SerializeField] private Button _generateButton;
        public Button GenerateButton { get => _generateButton; set => _generateButton = value; }

        [SerializeField] private Text _messageLabel;
        public Text MessageLabel { get => _messageLabel; set => _messageLabel = value; }

        [SerializeField] private Camera2DController _camera;
        public Camera2DController Camera { get => _camera; set => _camera = value; }

        private GameObject Container { get; set; }

        private void Awake()
        {
            GenerateButton.onClick.AddListener(OnGenerateButtonPressed);
        }

        private void OnGenerateButtonPressed()
        {
            GenerateLayoutAsync();
        }

        private async void GenerateLayoutAsync()
        {
            MessageLabel.text = "Generating...";
            GenerateButton.interactable = false;
            var seed = Random.Range(1, int.MaxValue);
            Pipeline.SetRandomSeed(seed);
            var token = new CancellationTokenSource(10000).Token;
            var result = await Pipeline.RunAsync();
            GenerateButton.interactable = true;

            if (!result.Success)
            {
                MessageLabel.text = $"Generation FAILED (Seed = {seed})";
                return;
            }

            MessageLabel.text = string.Empty;
            var layout = result.GetOutput<Layout>("Layout");
            var layoutPack = new LayoutPack(layout, new LayoutState(layout));
            CreateContainer();
            RoomTemplateDatabase.InstantiateAllRooms(layoutPack, Container.transform);
            Camera.ResetPosition();
        }

        private void CreateContainer()
        {
            if (Container != null)
                Destroy(Container);

            Container = new GameObject("Container");
            Container.transform.SetParent(transform);
        }
    }
}