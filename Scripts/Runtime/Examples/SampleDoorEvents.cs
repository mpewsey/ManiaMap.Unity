using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Examples
{
    [RequireComponent(typeof(Door))]
    public class SampleDoorEvents : MonoBehaviour
    {
        public Door Door { get; private set; }

        private void Awake()
        {
            Door = GetComponent<Door>();
            Door.OnInitialize.AddListener(OnInitialize);
        }

        private void OnInitialize(Door door)
        {
            if (!door.Exists)
                Destroy(door.gameObject);
        }
    }
}
