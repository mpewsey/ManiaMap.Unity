using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    public class GenerationNamedInput<T> : GenerationInput
    {
        [SerializeField]
        protected string _name = "<None>";
        public string Name { get => _name; set => _name = value; }

        [SerializeField]
        protected T _value;
        public T Value { get => _value; set => _value = value; }

        public override void AddInput(Dictionary<string, object> input)
        {
            input.Add(Name, Value);
        }
    }
}
