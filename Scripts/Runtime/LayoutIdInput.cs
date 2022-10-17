using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// A component for adding a layoud ID input to a GenerationPipeline.
    /// </summary>
    public class LayoutIdInput : GenerationInput
    {
        [SerializeField]
        private int _id;
        /// <summary>
        /// The layout ID.
        /// </summary>
        public int Id { get => _id; set => _id = value; }

        /// <inheritdoc/>
        public override void AddInput(Dictionary<string, object> input)
        {
            input.Add("LayoutId", Id);
        }

        /// <inheritdoc/>
        public override string[] OutputNames()
        {
            return new string[] { "LayoutId" };
        }
    }
}