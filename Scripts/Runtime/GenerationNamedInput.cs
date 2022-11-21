using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// The base component for adding a named input to a GenerationPipeline.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    public abstract class GenerationNamedInput<T> : GenerationInput
    {
        [SerializeField]
        protected string _name = "<None>";
        /// <summary>
        /// The input argument name.
        /// </summary>
        public string Name { get => _name; set => _name = value; }

        [SerializeField]
        protected T _value;
        /// <summary>
        /// The value.
        /// </summary>
        public T Value { get => _value; set => _value = value; }

        /// <inheritdoc/>
        public override void AddInputs(Dictionary<string, object> input)
        {
            input.Add(Name, Value);
        }
    }
}
