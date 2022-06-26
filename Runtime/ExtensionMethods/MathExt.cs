using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniKit
{
    public static class MathExt
    {
        public static int Modulu(this int a, int b)
                    => a - b * Mathf.FloorToInt((float)a / (float)b);

        public static void Shuffle<T>(this IList<T> list, object seed = null)
        {
            if (seed == null)
                seed = DateTime.Now;

            System.Random rand = new System.Random(seed.GetHashCode());

            for (int i = 0; i < list.Count - 1; i++)
            {
                int j = i + rand.Next(0, list.Count - i);

                //Swap
                (list[j], list[i]) = (list[i], list[j]);
            }
        }
    }
}
