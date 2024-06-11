using UnityEngine;

namespace MPewsey.ManiaMapUnity
{
    public static class Rand
    {
        public static int AutoAssignId(int id)
        {
            return id <= 0 ? GetRandomId() : id;
        }

        public static int GetRandomId()
        {
            return Random.Range(1, int.MaxValue);
        }
    }
}