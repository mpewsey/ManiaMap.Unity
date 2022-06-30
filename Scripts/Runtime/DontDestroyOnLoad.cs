using UnityEngine;

namespace MPewsey.ManiaMap.Unity
{
    /// <summary>
    /// Applies the DontDestroyOnLoad method to the attached Game Object on Awake.
    /// </summary>
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}