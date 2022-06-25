using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A component for generation layouts as part of a GenerationPipeline.
    /// </summary>
    public class LayoutGenerator : GenerationStep
    {
        [SerializeField]
        private int _maxRebases = 100;
        /// <summary>
        /// The maximum number of times a layout may be used as a base.
        /// </summary>
        public int MaxRebases { get => _maxRebases; set => _maxRebases = Mathf.Max(value, 1); }

        [SerializeField]
        private float _rebaseDecayRate = 0.25f;
        /// <summary>
        /// The decay rate applied to the maximum number of rebases.
        /// </summary>
        public float RebaseDecayRate { get => _rebaseDecayRate; set => _rebaseDecayRate = Mathf.Max(value, 0); }

        [SerializeField]
        private int _maxBranchLength = -1;
        /// <summary>
        /// The maximum length for graph branches. If less than or equal to zero, branches will not be split.
        /// </summary>
        public int MaxBranchLength { get => _maxBranchLength; set => _maxBranchLength = Mathf.Max(value, -1); }

        private void OnValidate()
        {
            MaxRebases = MaxRebases;
            RebaseDecayRate = RebaseDecayRate;
            MaxBranchLength = MaxBranchLength;
        }

        /// <inheritdoc/>
        public override IGenerationStep GetStep()
        {
            return new ManiaMap.LayoutGenerator(MaxRebases, RebaseDecayRate, MaxBranchLength);
        }

        /// <inheritdoc/>
        public override string[] InputNames()
        {
            return new string[] { "LayoutId", "LayoutGraph", "TemplateGroups", "RandomSeed" };
        }

        /// <inheritdoc/>
        public override string[] OutputNames()
        {
            return new string[] { "Layout" };
        }
    }
}
