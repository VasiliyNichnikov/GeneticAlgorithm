using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Vector3Utils
    {
        public static IEnumerable<Vector3Int> ToEnumerable(this Vector3Int vector)
        {
            yield return vector;
        }
    }
}