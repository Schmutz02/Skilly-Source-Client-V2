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
    }
}