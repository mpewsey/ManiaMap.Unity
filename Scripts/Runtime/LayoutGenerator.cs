using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class LayoutGenerator : GenerationStep
    {
        [SerializeField]
        private int _maxRebases = 100;
        public int MaxRebases { get => _maxRebases; set => _maxRebases = Mathf.Max(value, 1); }

        [SerializeField]
        private float _rebaseDecayRate = 0.25f;
        public float RebaseDecayRate { get => _rebaseDecayRate; set => _rebaseDecayRate = Mathf.Max(value, 0); }

        [SerializeField]
        private int _maxBranchLength = -1;
        public int MaxBranchLength { get => _maxBranchLength; set => _maxBranchLength = Mathf.Max(value, -1); }

        private void OnValidate()
        {
            MaxRebases = MaxRebases;
            RebaseDecayRate = RebaseDecayRate;
            MaxBranchLength = MaxBranchLength;
        }

        public override IGenerationStep GetStep()
        {
            return new ManiaMap.LayoutGenerator(MaxRebases, RebaseDecayRate, MaxBranchLength);
        }

        public override string[] InputNames()
        {
            return new string[] { "LayoutId", "LayoutGraph", "TemplateGroups", "RandomSeed" };
        }

        public override string[] OutputNames()
        {
            return new string[] { "Layout" };
        }
    }
}
