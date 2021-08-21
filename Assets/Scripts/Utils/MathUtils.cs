using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class MathUtils
    {
        public static float BoundToPI(float x)
        {
            int v;
            if (x < -Mathf.PI)
            {
                v = ((int)(x / -Mathf.PI) + 1) / 2;
                x += v * 2 * Mathf.PI;
            }
            else if (x > Mathf.PI)
            {
                v = ((int)(x / Mathf.PI) + 1) / 2;
                x -= v * 2 * Mathf.PI;
            }
            return x;
        }
        
        public static float DistanceSquared(Vector2 from, Vector2 to)
        {
            float v1 = from.x - to.x, v2 = from.y - to.y;
            return v1 * v1 + v2 * v2;
        }

        public static T Random<T>(this List<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
    }
}