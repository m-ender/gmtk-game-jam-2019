using System.Collections.Generic;
using UnityEngine;

namespace GMTKGJ2019
{
    public static class ExtensionMethods
    {
        // Custom deconstructions
        public static void Deconstruct(this Vector2Int vec, out int x, out int y)
        {
            x = vec.x;
            y = vec.y;
        }
        public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value)
        {
            key = tuple.Key;
            value = tuple.Value;
        }
    }
}