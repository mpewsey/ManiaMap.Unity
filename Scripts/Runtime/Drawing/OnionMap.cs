using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Drawing
{
    public class OnionMap : MonoBehaviour
    {
        [SerializeField]
        private float _drawDepth = 1;
        public float DrawDepth { get => _drawDepth; set => _drawDepth = value; }

        [SerializeField]
        private Gradient _gradient = DefaultGradient();
        public Gradient Gradient { get => _gradient; set => _gradient = value; }

        private float _position = -1e20f;
        public float Position { get => _position; set => SetPosition(value); }

        public IOnionMapTarget Target { get; private set; }

        private void Awake()
        {
            Target = GetComponent<IOnionMapTarget>();
        }

        public static Gradient DefaultGradient()
        {
            var gradient = new Gradient();

            var colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(Color.red, 0),
                new GradientColorKey(Color.white, 0.45f),
                new GradientColorKey(Color.white, 0.55f),
                new GradientColorKey(Color.blue, 1),
            };

            var alphaKeys = new GradientAlphaKey[]
            {
                new GradientAlphaKey(0, 0),
                new GradientAlphaKey(1, 0.45f),
                new GradientAlphaKey(1, 0.55f),
                new GradientAlphaKey(0, 1),
            };

            gradient.SetKeys(colorKeys, alphaKeys);
            return gradient;
        }

        private void SetPosition(float value)
        {
            var oldValue = _position;
            _position = ClampPosition(value);

            if (!Mathf.Approximately(_position, oldValue))
                Apply();
        }

        private float ClampPosition(float value)
        {
            var range = Target.LayerRange();
            return Mathf.Clamp(value, range.x, range.y);
        }

        public void Apply()
        {
            var scale = 0.5f / DrawDepth;

            foreach (var layer in Target.Layers())
            {
                var t = (layer.Position() - Position) * scale + 0.5f;
                layer.Apply(Gradient.Evaluate(t));
            }
        }
    }
}